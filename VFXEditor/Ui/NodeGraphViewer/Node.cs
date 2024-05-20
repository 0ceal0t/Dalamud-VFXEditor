using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer {
    public abstract class Node {
        private static int NODE_ID = 0;
        public static readonly Vector2 NodeInsidePadding = new( 3.5f, 3.5f );
        public static readonly Vector2 MinHandleSize = new( 50, 20 );
        public static readonly float HandleButtonBoxItemWidth = 20;

        public readonly int Id = NODE_ID++;

        public string Header { get; protected set; }

        public NodeStyle Style = new( Vector2.Zero, Vector2.Zero );

        protected virtual Vector2 RecommendedInitSize => HasBody ? new( 100, 200 ) : new( 0, 0 );
        protected Vector2? LastUnminimizedSize = null;

        protected readonly bool HasBody;
        protected readonly bool ShowControls;

        public bool IsMarkedDeleted = false;
        public bool IsBusy = false;

        protected bool IsMinimized = false;
        protected bool NeedReinit = false;

        public Node( string header, bool hasBody, bool showControls ) {
            Header = header;
            HasBody = hasBody;
            ShowControls = showControls;
            // Basically SetHeader() and AdjustSizeToContent(),
            // but we need non-ImGui option for loading out of Draw()
            if( Plugin.IsImguiSafe ) SetHeader( Header );
            else {
                Style.SetHandleTextSize( new Vector2( Header.Length * 6, 11 ) );
                Style.SetSize( new Vector2( Header.Length * 6, 11 ) + NodeInsidePadding * 2 );
                NeedReinit = true;
            }

            Style.SetSize( RecommendedInitSize );
        }

        protected virtual void ReInit() {
            SetHeader( Header );               // adjust minSize to new header
            Style.SetSize( RecommendedInitSize );        // adjust size to the new minSize
        }

        public abstract void OnDelete();

        public virtual void SetHeader( string header, bool pAutoSizing = true, bool pAdjustWidthOnly = false, bool pChooseGreaterWidth = false ) {
            Header = header;
            Style.SetHandleTextSize( ImGui.CalcTextSize( Header ) );
            if( pAutoSizing ) AdjustSizeToHeader( pAdjustWidthOnly, pChooseGreaterWidth );
        }

        public virtual void AdjustSizeToHeader( bool pAdjustWidthOnly = false, bool pChooseGreaterWidth = false ) {
            if( pAdjustWidthOnly ) {
                var tW = ( ImGui.CalcTextSize( Header ) + NodeInsidePadding * 2 ).X;
                Style.SetSize( new( pChooseGreaterWidth ? ( tW > Style.GetSize().X ? tW : Style.GetSize().X ) : tW, Style.GetSize().Y ) );
            }
            else Style.SetSize( ImGui.CalcTextSize( Header ) + NodeInsidePadding * 2 );
        }

        public void Minimize() => IsMinimized = true;

        public void Unminimize() => IsMinimized = false;

        public NodeInteractionFlags Draw( Vector2 nodePos, float canvasScaling, bool isActive, InputPayload inputPayload, bool thisNodeConnecting, bool someNodeConnecting ) {
            using var _ = ImRaii.PushId( Id );

            // Re-calculate ImGui-dependant members, if required.
            if( NeedReinit ) {
                ReInit();
                NeedReinit = false;
            }
            // Minimize/Unminimize
            if( IsMinimized && !LastUnminimizedSize.HasValue ) {
                LastUnminimizedSize = Style.GetSize();
                Style.SetSize( new Vector2( LastUnminimizedSize.Value.X, 0 ) );
            }
            else if( !IsMinimized && LastUnminimizedSize.HasValue ) {
                Style.SetSize( LastUnminimizedSize.Value );
                LastUnminimizedSize = null;
            }

            var tNodeSize = HasBody ? Style.GetSizeScaled( canvasScaling ) : Style.GetHandleSizeScaled( canvasScaling );
            Vector2 tOuterWindowSizeOfs = new( 15 * canvasScaling );
            var tEnd = nodePos + tNodeSize;
            var tRes = NodeInteractionFlags.None;

            // resize grip
            if( !IsMinimized && HasBody ) {
                Vector2 tGripSize = new( 10, 10 );
                tGripSize *= canvasScaling * 0.8f;     // making this scale less
                ImGui.SetCursorScreenPos( tEnd - tGripSize * ( isActive ? 0.425f : 0.57f ) );
                ImGui.PushStyleColor( ImGuiCol.Button, ImGui.ColorConvertFloat4ToU32( Style.ColorFg ) );
                ImGui.Button( $"##{Id}", tGripSize );
                if( ImGui.IsItemHovered() ) { ImGui.SetMouseCursor( ImGuiMouseCursor.ResizeNWSE ); }
                if( ImGui.IsItemActive() ) {
                    IsBusy = true;
                    Style.SetSizeScaled( inputPayload.mMousePos - nodePos, canvasScaling );
                }
                if( IsBusy && !inputPayload.mIsMouseLmbDown ) { IsBusy = false; }
                ImGui.PopStyleColor();
                ImGui.SetCursorScreenPos( nodePos );
            }

            // Each node drawing have 2 child windows.
            // One to get this node's drawList so that it would take priority over master drawlist.
            // One to format the node content.
            ImGui.SetCursorScreenPos( nodePos - tOuterWindowSizeOfs / 2 );
            ImGui.BeginChild(
                $"##outer{Id}", tNodeSize + tOuterWindowSizeOfs, false,
                ImGuiWindowFlags.NoMouseInputs | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoScrollbar );
            var drawList = ImGui.GetWindowDrawList();

            ImGui.SetCursorScreenPos( nodePos );
            // outline
            drawList.AddRect(
                nodePos,
                tEnd,
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorFg, isActive ? 0.7f : 0.2f ) ),
                1,
                ImDrawFlags.None,
                ( isActive ? 6.5f : 4f ) * canvasScaling );

            //node content(handle, body)
            NodeUtils.PushFontScale( canvasScaling );
            ImGui.PushStyleVar( ImGuiStyleVar.WindowPadding, NodeInsidePadding * canvasScaling );
            ImGui.PushStyleVar( ImGuiStyleVar.ScrollbarSize, 1.5f );
            ImGui.PushStyleColor( ImGuiCol.ChildBg, ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            ImGui.PushStyleColor( ImGuiCol.Border, NodeUtils.AdjustTransparency( Style.ColorFg, isActive ? 0.7f : 0.2f ) );

            ImGui.BeginChild( $"{Id}", tNodeSize, border: true, ImGuiWindowFlags.ChildWindow );
            // backdrop (leave this here so the backgrop can overwrite the child's bg)
            if( !IsMinimized ) drawList.AddRectFilled( nodePos, tEnd, ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            tRes |= DrawHandle( nodePos, canvasScaling, drawList, isActive );

            if( HasBody ) {
                ImGui.SetCursorScreenPos( new Vector2( nodePos.X + 2 * canvasScaling, ImGui.GetCursorScreenPos().Y + 5 * canvasScaling ) );
                if( !IsMinimized ) tRes |= DrawBody( nodePos, canvasScaling );
            }

            ImGui.EndChild();

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
            NodeUtils.PopFontScale();

            ImGui.EndChild();

            tRes |= DrawConnection( drawList, nodePos, isActive, true, thisNodeConnecting, someNodeConnecting );
            tRes |= DrawConnection( drawList, nodePos, isActive, false, thisNodeConnecting, someNodeConnecting );

            return tRes;
        }

        protected virtual NodeInteractionFlags DrawHandle( Vector2 pNodeOSP, float canvasScaling, ImDrawListPtr pDrawList, bool pIsActive ) {
            var handleSize = Style.GetHandleSizeScaled( canvasScaling );
            pDrawList.AddRectFilled(
                pNodeOSP,
                pNodeOSP + handleSize,
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorUnique, pIsActive ? 0.45f : 0.15f ) ) );

            ImGui.TextColored( NodeUtils.Colors.NodeText, Header );

            if( ShowControls ) {
                // ButtonBox
                ImGui.SameLine();
                NodeUtils.AlignRight( HandleButtonBoxItemWidth * 2 * canvasScaling, pConsiderImguiPaddings: false );
                return DrawHandleButtonBox( canvasScaling );
            }

            return NodeInteractionFlags.None;
        }

        protected abstract NodeInteractionFlags DrawBody( Vector2 pNodeOSP, float pCanvasScaling );

        protected NodeInteractionFlags DrawHandleButtonBox( float pCanvasScaling ) {
            var tRes = NodeInteractionFlags.None;
            ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 0 ) );
            ImGui.PushStyleColor( ImGuiCol.Text, NodeUtils.Colors.NodeText );

            Vector2 tBSize = new( HandleButtonBoxItemWidth, Style.GetHandleSize().Y * 0.8f );
            tBSize *= pCanvasScaling;
            if( ImGui.Selectable( IsMinimized ? "  ▲" : "  ▼", false, ImGuiSelectableFlags.DontClosePopups, tBSize ) ) {
                IsMinimized = !IsMinimized;
            }
            if( ImGui.IsItemActive() ) tRes |= NodeInteractionFlags.Internal;
            ImGui.SameLine();
            if( ImGui.Selectable( " ×", false, ImGuiSelectableFlags.DontClosePopups, tBSize ) ) {
                IsMarkedDeleted = true;
            }
            if( ImGui.IsItemActive() ) tRes |= NodeInteractionFlags.Internal;

            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
            return tRes;
        }

        public NodeInteractionFlags DrawConnection( ImDrawListPtr drawList, Vector2 nodePosition, bool active, bool isInput, bool thisNodeConnecting, bool someNodeConnecting ) {
            using var _ = ImRaii.PushId( isInput ? "Input" : "Output" );

            var position = isInput ? nodePosition : GetOutputPosition( nodePosition );
            var tRes = NodeInteractionFlags.None;
            Vector2 tSize = new( 5f, 5f );
            var tIsHovered = false;
            var cursor = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos( position - ( tSize * 3f ) / 2f );
            if( ImGui.InvisibleButton( $"##Button", tSize * 3f, ImGuiButtonFlags.MouseButtonRight | ImGuiButtonFlags.MouseButtonLeft ) ) {
                if( ImGui.GetIO().MouseReleased[0] ) {
                    if( isInput ) {
                        tRes |= ( !someNodeConnecting ) ? NodeInteractionFlags.UnrequestingEdgeConn : NodeInteractionFlags.RequestingEdgeConn;
                    }
                    else {
                        tRes |= ( someNodeConnecting && !thisNodeConnecting ) ? NodeInteractionFlags.UnrequestingEdgeConn : NodeInteractionFlags.RequestingEdgeConn;
                    }
                }

            }
            else if( ImGui.IsItemHovered() ) {
                tIsHovered = true;
            }

            ImGui.SetCursorScreenPos( cursor );
            drawList.AddCircleFilled(
                position, tSize.X * ( ( tIsHovered || thisNodeConnecting ) ? 2f : 1 ),
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency(
                    NodeUtils.Colors.NodeFg,
                    ( active || tIsHovered || thisNodeConnecting ) ? 1f : 0.7f ) ) );
            drawList.AddCircleFilled( position, ( tSize.X * 0.7f ) * ( ( tIsHovered || thisNodeConnecting ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            drawList.AddCircleFilled( position, ( tSize.X * 0.5f ) * ( ( tIsHovered || thisNodeConnecting ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorUnique, ( active || thisNodeConnecting ) ? 0.55f : 0.25f ) ) );
            return tRes;
        }

        public Vector2 GetOutputPosition( Vector2 nodePosition ) => nodePosition + Style.GetHandleSize();

        public abstract void Dispose();
    }
}
