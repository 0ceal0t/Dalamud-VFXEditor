using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Canvas;
using VfxEditor.Ui.NodeGraphViewer.Content;

namespace VfxEditor.Ui.NodeGraphViewer {
    public class NodeGraphViewer {
        private static Vector2 RecommendedViewerSizeToSearch = new( 200, 300 );

        public readonly NodeCanvas ActiveCanvas;

        private bool IsMouseHoldingViewer = false;
        private bool IsShowingRulerText = false;
        private DateTime? RulerTextLastAppear = null;
        public Vector2? Size = null;
        private Vector2? SizeLastKnown = null;
        private string InfieldNodeLookupVal = "";

        public NodeGraphViewer() {
            ActiveCanvas = new( 0 );
        }

        public void AddNodeToActiveCanvas<T>( NodeContent pNodeContent ) where T : Node, new() => ActiveCanvas.AddNodeWithinView<T>( pNodeContent, SizeLastKnown ?? RecommendedViewerSizeToSearch );

        public void Draw( HashSet<ImGuiKey> pExtraKeyboardInputs = null ) => Draw( ImGui.GetCursorScreenPos(), pExtraKeyboardInputs: pExtraKeyboardInputs );

        public void Draw( Vector2 pScreenPos, Vector2? pSize = null, HashSet<ImGuiKey> pExtraKeyboardInputs = null ) {
            SizeLastKnown = pSize ?? ImGui.GetContentRegionAvail();
            Area tGraphArea = new( pScreenPos + new Vector2( 0, 30 ), ( SizeLastKnown ?? ImGui.GetContentRegionAvail() ) + new Vector2( 0, -30 ) );
            var tDrawList = ImGui.GetWindowDrawList();

            DrawUtilsBar();
            ImGui.SetCursorScreenPos( tGraphArea.start );
            DrawGraph( tGraphArea, tDrawList, pExtraKeyboardInputs: pExtraKeyboardInputs );
        }

        private void DrawUtilsBar() {
            var tScaling = ( int )( ActiveCanvas.GetScaling() * 100 );
            ImGui.SetNextItemWidth( 35 );
            if( ImGui.DragInt( "##sldScaling", ref tScaling, NodeCanvas.StepScale * 100, ( int )( NodeCanvas.MinScale * 100 ), ( int )( NodeCanvas.MaxScale * 100 ), "%d%%" ) ) {
                ActiveCanvas.SetScaling( ( float )tScaling / 100 );
            }

            ImGui.SameLine();
            DrawNodeSearchBox( pTextBoxWidth: 200 );
        }

        private void DrawGraph( Area pGraphArea, ImDrawListPtr pDrawList, HashSet<ImGuiKey> pExtraKeyboardInputs = null ) {
            ImGui.BeginChild(
                "nodegraphviewer",
                pGraphArea.size,
                border: true,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoMove );
            pDrawList.PushClipRect( pGraphArea.start, pGraphArea.end, true );

            var tSnapData = DrawGraphBg( pGraphArea, ActiveCanvas.GetBaseOffset(), ActiveCanvas.GetScaling() );
            DrawGraphNodes( pGraphArea, tSnapData, pDrawList, pExtraKeyboardInputs: pExtraKeyboardInputs );
            ImGui.EndChild();
            pDrawList.PopClipRect();
        }

        private void DrawGraphNodes( Area pGraphArea, GridSnapData pSnapData, ImDrawListPtr pDrawList, HashSet<ImGuiKey> pExtraKeyboardInputs = null ) {
            ImGui.SetCursorScreenPos( pGraphArea.start );

            // check if mouse within viewer, and if mouse is holding on viewer.
            InputPayload tInputPayload = new();
            tInputPayload.CaptureInput( pExtraKeyboardInputs: pExtraKeyboardInputs );
            var tIsWithinViewer = pGraphArea.CheckPosIsWithin( tInputPayload.mMousePos );
            IsMouseHoldingViewer = tInputPayload.mIsMouseLmbDown && ( tIsWithinViewer || IsMouseHoldingViewer );

            if( tIsWithinViewer ) { tInputPayload.CaptureMouseWheel(); }
            if( IsMouseHoldingViewer ) { tInputPayload.CaptureMouseDragDelta(); }

            var tRes = ActiveCanvas.Draw(
                                    pGraphArea.center,
                                    pGraphArea.start,
                                    pGraphArea.size,
                                    -1 * pGraphArea.size / 2,
                                    Plugin.Configuration.NodeGraphGridSnapProximity,
                                    tInputPayload,
                                    pDrawList,
                                    pSnapData: pSnapData,
                                    pCanvasDrawFlag: ( ImGui.IsWindowFocused( ImGuiFocusedFlags.ChildWindows ) && ( IsMouseHoldingViewer || tIsWithinViewer ) )
                                                     ? CanvasDrawFlags.None
                                                     : CanvasDrawFlags.NoInteract
                                    );
            if( tRes.HasFlag( CanvasDrawFlags.StateNodeDrag ) || tRes.HasFlag( CanvasDrawFlags.StateCanvasDrag ) ) {
                IsShowingRulerText = true;
                RulerTextLastAppear = DateTime.Now;
            }
            // Snap lines
            if( !tRes.HasFlag( CanvasDrawFlags.NoNodeSnap ) ) DrawSnapLine( pGraphArea, pSnapData );
        }

        private GridSnapData DrawGraphBg( Area pArea, Vector2 pOffset, float pCanvasScale ) {
            GridSnapData tGridSnap = new();
            var tUGSmall = Plugin.Configuration.NodeGraphUnitGridSmall * pCanvasScale;
            var tUGLarge = Plugin.Configuration.NodeGraphUnitGridLarge * pCanvasScale;
            ImGui.SetCursorScreenPos( pArea.start );
            var pDrawList = ImGui.GetWindowDrawList();

            // Grid only adjusts to half of viewer size change,
            // When the viewer's size change, its midpoint only moves a distance of half the size change.
            // The canvas is anchored/offseted to the midpoint of viewer. Hence the canvas also moves half of size change.
            // And the grid should move along with the canvas (grid displays canvas's plane afterall, not the viewer),
            // honestly good luck with this.
            var tGridStart_S = pArea.start + new Vector2(
                        ( ( pOffset.X * pCanvasScale + ( pArea.size.X * 0.5f ) ) % tUGSmall ),
                        ( ( pOffset.Y * pCanvasScale + ( pArea.size.Y * 0.5f ) ) % tUGSmall )
                    );
            var tGridStart_L = pArea.start + new Vector2(
                        ( ( pOffset.X * pCanvasScale + ( pArea.size.X * 0.5f ) ) % tUGLarge ),
                        ( ( pOffset.Y * pCanvasScale + ( pArea.size.Y * 0.5f ) ) % tUGLarge )
                    );

            // backdrop
            pDrawList.AddRectFilled( pArea.start, pArea.end, ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( NodeUtils.Colors.NormalBar_Grey, 0.13f ) ) );

            // grid
            var tGridColor = ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( NodeUtils.Colors.NormalBar_Grey, 0.05f ) );
            for( var i = 0; i < ( pArea.end.X - tGridStart_S.X ) / tUGSmall; i++ )        // vertical S
            {
                pDrawList.AddLine( new Vector2( tGridStart_S.X + i * tUGSmall, pArea.start.Y ), new Vector2( tGridStart_S.X + i * tUGSmall, pArea.end.Y ), tGridColor, 1.0f );
            }
            for( var i = 0; i < ( pArea.end.Y - tGridStart_S.Y ) / tUGSmall; i++ )        // horizontal S
            {
                pDrawList.AddLine( new Vector2( pArea.start.X, tGridStart_S.Y + i * tUGSmall ), new Vector2( pArea.end.X, tGridStart_S.Y + i * tUGSmall ), tGridColor, 1.0f );
            }

            var tXFirstNotation = ( int )( -pOffset.X * pCanvasScale - pArea.size.X / 2 ) / ( int )tUGLarge * ( int )Plugin.Configuration.NodeGraphUnitGridLarge;
            var tYFirstNotation = ( int )( -pOffset.Y * pCanvasScale - pArea.size.Y / 2 ) / ( int )tUGLarge * ( int )Plugin.Configuration.NodeGraphUnitGridLarge;
            var tTransMax = 0.2f;
            for( var i = 0; i < ( pArea.end.X - tGridStart_L.X ) / tUGLarge; i++ )        // vertical L
            {
                pDrawList.AddLine( new Vector2( tGridStart_L.X + i * tUGLarge, pArea.start.Y ), new Vector2( tGridStart_L.X + i * tUGLarge, pArea.end.Y ), tGridColor, 2.0f );
                tGridSnap.X.Add( tGridStart_L.X + i * tUGLarge );
                if( IsShowingRulerText ) {
                    float tTrans = 1;
                    if( RulerTextLastAppear.HasValue )
                        tTrans = tTransMax - ( ( float )( ( DateTime.Now - RulerTextLastAppear.Value ).TotalMilliseconds ) / Plugin.Configuration.NodeGraphTimeForRulerTextFade ) * tTransMax;
                    pDrawList.AddText(
                        new Vector2( tGridStart_L.X + i * tUGLarge, pArea.start.Y ),
                        ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( NodeUtils.Colors.NodeText, tTrans ) ),
                        $"{( tXFirstNotation + ( Plugin.Configuration.NodeGraphUnitGridLarge * i ) ) / 10}" );
                    // fade check
                    if( tTrans < 0.05f ) {
                        RulerTextLastAppear = null;
                        IsShowingRulerText = false;
                    }
                }
            }
            for( var i = 0; i < ( pArea.end.Y - tGridStart_L.Y ) / tUGLarge; i++ )        // horizontal L
            {
                pDrawList.AddLine( new Vector2( pArea.start.X, tGridStart_L.Y + i * tUGLarge ), new Vector2( pArea.end.X, tGridStart_L.Y + i * tUGLarge ), tGridColor, 2.0f );
                tGridSnap.Y.Add( tGridStart_L.Y + i * tUGLarge );
                if( IsShowingRulerText ) {
                    float tTrans = 1;
                    if( RulerTextLastAppear.HasValue )
                        tTrans = tTransMax - ( ( float )( ( DateTime.Now - RulerTextLastAppear.Value ).TotalMilliseconds ) / Plugin.Configuration.NodeGraphTimeForRulerTextFade ) * tTransMax;
                    pDrawList.AddText(
                        new Vector2( pArea.start.X + 6, tGridStart_L.Y + i * tUGLarge ),
                        ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( NodeUtils.Colors.NodeText, tTrans ) ),
                        $"{( tYFirstNotation + ( Plugin.Configuration.NodeGraphUnitGridLarge * i ) ) / 10}" );
                    // fade check
                    if( tTrans < 0.05f ) {
                        RulerTextLastAppear = null;
                        IsShowingRulerText = false;
                    }
                }
            }

            return tGridSnap;
        }

        private static void DrawSnapLine( Area pGraphArea, GridSnapData pSnapData ) {
            var pDrawList = ImGui.GetWindowDrawList();
            // X
            if( pSnapData.lastClosestSnapX != null ) {
                pDrawList.AddLine(
                    new Vector2( pSnapData.lastClosestSnapX.Value, pGraphArea.start.Y ),
                    new Vector2( pSnapData.lastClosestSnapX.Value, pGraphArea.end.Y ),
                    ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( NodeUtils.Colors.NodeGraphViewer_SnaplineGold, 0.5f ) ),
                    1.0f );
            }
            // Y
            if( pSnapData.lastClosestSnapY != null ) {
                pDrawList.AddLine(
                    new Vector2( pGraphArea.start.X, pSnapData.lastClosestSnapY.Value ),
                    new Vector2( pGraphArea.end.X, pSnapData.lastClosestSnapY.Value ),
                    ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( NodeUtils.Colors.NodeGraphViewer_SnaplineGold, 0.5f ) ),
                    1.0f );
            }
        }

        private void DrawNodeSearchBox( Vector2? pPUSize = null, float? pTextBoxWidth = null ) {
            var tAnchor = ImGui.GetCursorScreenPos();

            if( pTextBoxWidth != null ) ImGui.SetNextItemWidth( pTextBoxWidth.Value );
            ImGui.InputTextWithHint( "", "Search...", ref InfieldNodeLookupVal, 200 );
            var tIsInputActive = ImGui.IsItemActive();
            var tIsItemPUOpened = false;

            if( InfieldNodeLookupVal.Length != 0 && tIsInputActive ) ImGui.OpenPopup( "##searchNodePU" );

            ImGui.SetNextWindowPos( tAnchor + new Vector2( 0, 25 ) );
            ImGui.SetNextWindowSizeConstraints( new Vector2( 50, 25 ), pPUSize ?? new Vector2( 300, 300 ) );

            if( ImGui.BeginPopup( "##searchNodePU", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.ChildWindow ) ) {
                var tNodeIds = ActiveCanvas.LookUpNode( InfieldNodeLookupVal );

                foreach( var nid in tNodeIds ) {
                    var node = ActiveCanvas.GetNode( nid );
                    if( node == null ) continue;

                    if( ImGui.Selectable( node.Content.GetHeader(), false, ImGuiSelectableFlags.DontClosePopups ) ) {
                        FocusOnNodeId_ActiveCanvas( node.Id );
                    }
                }
                if( !tIsInputActive && !ImGui.IsWindowFocused() && !tIsItemPUOpened ) ImGui.CloseCurrentPopup();
                ImGui.EndPopup();
            }
        }

        private void FocusOnNodeId_ActiveCanvas( string pNodeId, Vector2? pExtraOfs = null ) => ActiveCanvas.FocusOnNode( pNodeId, pExtraOfs );

        public void FocusOnNodeTag_ActiveCanvas( string pTag, Vector2? pExtraOfs = null ) {
            var tRes = ActiveCanvas.LookUpNodeWithTag( pTag );
            if( tRes.Count == 0 ) return;
            ActiveCanvas.FocusOnNode( tRes.First(), pExtraOfs );
        }

        public void Dispose() {
            ActiveCanvas.Dispose();
        }
    }
}
