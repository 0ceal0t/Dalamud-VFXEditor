using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Canvas;
using VfxEditor.Ui.NodeGraphViewer.Content;

namespace VfxEditor.Ui.NodeGraphViewer {
    public abstract class Node {
        public static readonly Vector2 NodeInsidePadding = new( 3.5f, 3.5f );
        public static readonly Vector2 MinHandleSize = new( 50, 20 );
        public static readonly float HandleButtonBoxItemWidth = 20;
        protected bool NeedReinit = false;
        public bool IsMarkedDeleted = false;

        public HashSet<string> Pack = [];           // represents the nodes that this node packs
        public string PackerNodeId = null;        // represents the node which packs this node (the master packer, not just the parent node). Only ONE packer per node.
        public bool IsPacked = false;
        public string Tag = null;
        public PackingStatus Packing = PackingStatus.None;
        public Vector2? RelaPosLastPackingCall = null;

        public abstract string Type { get; }
        public string Id { get; protected set; } = string.Empty;
        public int GraphId = -1;
        protected Queue<Seed> Seeds = new();

        public NodeContent Content = new();
        public NodeStyle Style = new( Vector2.Zero, Vector2.Zero );
        protected virtual Vector2 RecommendedInitSize { get; } = new( 100, 200 );
        protected bool IsMinimized = false;
        public bool IsBusy = false;
        protected Vector2? LastUnminimizedSize = null;

        public Node() { }

        public virtual void Init( string pNodeId, int pGraphId, NodeContent pContent, NodeStyle _style = null, string pTag = null ) {
            Id = pNodeId;
            Tag = pTag;
            GraphId = pGraphId;
            Content = pContent;
            if( _style != null ) Style = _style;

            // Basically SetHeader() and AdjustSizeToContent(),
            // but we need non-ImGui option for loading out of Draw()
            if( Plugin.IsImguiSafe ) {
                SetHeader( Content.GetHeader() );
            }
            else {
                Content._setHeader( Content.GetHeader() );
                Style.SetHandleTextSize( new Vector2( Content.GetHeader().Length * 6, 11 ) );
                Style.SetSize( new Vector2( Content.GetHeader().Length * 6, 11 ) + NodeInsidePadding * 2 );
                NeedReinit = true;
            }

            Style.SetSize( RecommendedInitSize );
        }

        protected virtual void ReInit() {
            SetHeader( Content.GetHeader() );               // adjust minSize to new header
            Style.SetSize( RecommendedInitSize );        // adjust size to the new minSize
        }

        public abstract void OnDelete();

        public void SetId( string pId ) => Id = pId;

        public virtual void SetHeader( string pText, bool pAutoSizing = true, bool pAdjustWidthOnly = false, bool pChooseGreaterWidth = false ) {
            Content._setHeader( pText );
            Style.SetHandleTextSize( ImGui.CalcTextSize( Content.GetHeader() ) );
            if( pAutoSizing ) AdjustSizeToHeader( pAdjustWidthOnly, pChooseGreaterWidth );
        }

        public virtual void AdjustSizeToHeader( bool pAdjustWidthOnly = false, bool pChooseGreaterWidth = false ) {
            if( pAdjustWidthOnly ) {
                var tW = ( ImGui.CalcTextSize( Content.GetHeader() ) + NodeInsidePadding * 2 ).X;
                Style.SetSize(
                    new(
                        pChooseGreaterWidth
                        ? tW > Style.GetSize().X
                            ? tW
                            : Style.GetSize().X
                        : tW,
                        Style.GetSize().Y
                        )
                    );
            }
            else
                Style.SetSize( ImGui.CalcTextSize( Content.GetHeader() ) + NodeInsidePadding * 2 );
        }

        public virtual Seed GetSeed() => Seeds.Count == 0 ? null : Seeds.Dequeue();

        protected virtual void SetSeed( Seed pSeed ) => Seeds.Enqueue( pSeed );

        public void Minimize() => IsMinimized = true;

        public void Unminimize() => IsMinimized = false;

        public NodeInteractionFlags Draw(
            Vector2 pNodeOSP,
            float pCanvasScaling,
            bool pIsActive,
            InputPayload pInputPayload,
            bool pIsEstablishingConn = false,
            bool pIsDrawingHndCtxMnu = false ) {
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

            var tNodeSize = Style.GetSizeScaled( pCanvasScaling );
            Vector2 tOuterWindowSizeOfs = new( 15 * pCanvasScaling );
            var tEnd = pNodeOSP + tNodeSize;
            var tRes = NodeInteractionFlags.None;

            // resize grip
            if( !IsMinimized ) {
                Vector2 tGripSize = new( 10, 10 );
                tGripSize *= pCanvasScaling * 0.8f;     // making this scale less
                ImGui.SetCursorScreenPos( tEnd - tGripSize * ( pIsActive ? 0.425f : 0.57f ) );
                ImGui.PushStyleColor( ImGuiCol.Button, ImGui.ColorConvertFloat4ToU32( Style.ColorFg ) );
                ImGui.Button( $"##{Id}", tGripSize );
                if( ImGui.IsItemHovered() ) { ImGui.SetMouseCursor( ImGuiMouseCursor.ResizeNWSE ); }
                //else { ImGui.SetMouseCursor(ImGuiMouseCursor.Arrow); }
                if( ImGui.IsItemActive() ) {
                    IsBusy = true;
                    Style.SetSizeScaled( pInputPayload.mMousePos - pNodeOSP, pCanvasScaling );
                }
                if( IsBusy && !pInputPayload.mIsMouseLmbDown ) { IsBusy = false; }
                ImGui.PopStyleColor();
                ImGui.SetCursorScreenPos( pNodeOSP );
            }

            // Each node drawing have 2 child windows.
            // One to get this node's drawList so that it would take priority over master drawlist.
            // One to format the node content.
            ImGui.SetCursorScreenPos( pNodeOSP - tOuterWindowSizeOfs / 2 );
            ImGui.BeginChild(
                $"##outer{Id}", tNodeSize + tOuterWindowSizeOfs, false,
                ImGuiWindowFlags.NoMouseInputs | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoScrollbar );
            var tDrawList = ImGui.GetWindowDrawList();

            ImGui.SetCursorScreenPos( pNodeOSP );
            // outline
            tDrawList.AddRect(
                pNodeOSP,
                tEnd,
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorFg, pIsActive ? 0.7f : 0.2f ) ),
                1,
                ImDrawFlags.None,
                ( pIsActive ? 6.5f : 4f ) * pCanvasScaling );

            //node content(handle, body)
            NodeUtils.PushFontScale( pCanvasScaling );
            ImGui.PushStyleVar( ImGuiStyleVar.WindowPadding, NodeInsidePadding * pCanvasScaling );
            ImGui.PushStyleVar( ImGuiStyleVar.ScrollbarSize, 1.5f );
            ImGui.PushStyleColor( ImGuiCol.ChildBg, ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            ImGui.PushStyleColor( ImGuiCol.Border, NodeUtils.AdjustTransparency( Style.ColorFg, pIsActive ? 0.7f : 0.2f ) );

            ImGui.BeginChild(
                Id,
                tNodeSize,
                border: true,
                ImGuiWindowFlags.ChildWindow
                );
            // backdrop (leave this here so the backgrop can overwrite the child's bg)
            if( !IsMinimized ) tDrawList.AddRectFilled( pNodeOSP, tEnd, ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            tRes |= DrawHandle( pNodeOSP, pCanvasScaling, tDrawList, pIsActive );
            ImGui.SetCursorScreenPos( new Vector2( pNodeOSP.X + 2 * pCanvasScaling, ImGui.GetCursorScreenPos().Y + 5 * pCanvasScaling ) );
            if( !IsMinimized ) tRes |= DrawBody( pNodeOSP, pCanvasScaling );
            ImGui.EndChild();

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
            NodeUtils.PopFontScale();

            ImGui.EndChild();

            // HndCtxMnu
            if( pIsDrawingHndCtxMnu ) ImGui.OpenPopup( GetExtraOptionPUGuiId() );
            ImGui.PushStyleColor( ImGuiCol.Text, NodeUtils.Colors.NodeText );
            ImGui.PopStyleColor();

            tRes |= DrawEdgePlugButton( tDrawList, pNodeOSP, pIsActive, pIsEstablishingConn: pIsEstablishingConn );

            return tRes;
        }

        protected virtual NodeInteractionFlags DrawHandle( Vector2 pNodeOSP, float pCanvasScaling, ImDrawListPtr pDrawList, bool pIsActive ) {
            var tHandleSize = Style.GetHandleSizeScaled( pCanvasScaling );
            pDrawList.AddRectFilled(
                pNodeOSP,
                pNodeOSP + tHandleSize,
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorUnique, pIsActive ? 0.45f : 0.15f ) ) );

            ImGui.TextColored( NodeUtils.Colors.NodeText, Content.GetHeader() );

            // ButtonBox
            ImGui.SameLine();
            NodeUtils.AlignRight( HandleButtonBoxItemWidth * 3 * pCanvasScaling, pConsiderImguiPaddings: false );

            var tRes = DrawHandleButtonBox( pCanvasScaling );
            return tRes;
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

        protected string GetExtraOptionPUGuiId() => $"##hepu{Id}";

        public NodeInteractionFlags DrawEdgePlugButton( ImDrawListPtr pDrawList, Vector2 pNodeOSP, bool pIsActive, bool pIsEstablishingConn = false ) {
            var tRes = NodeInteractionFlags.None;
            Vector2 tSize = new( 5f, 5f );
            var tIsHovered = false;
            var tOriAnchor = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos( pNodeOSP - ( tSize * 3f ) );
            if( ImGui.InvisibleButton( $"eb{Id}", tSize * 3f, ImGuiButtonFlags.MouseButtonRight | ImGuiButtonFlags.MouseButtonLeft ) ) {
                // LMB: Collapse child nodes
                if( ImGui.GetIO().MouseReleased[0] ) {
                    Packing = Packing switch {
                        PackingStatus.PackingDone => PackingStatus.UnpackingUnderway,
                        PackingStatus.None => PackingStatus.PackingUnderway,
                        _ => Packing
                    };
                }
                // RMB: Node conn
                else if( ImGui.GetIO().MouseReleased[1] ) {
                    tRes |= pIsEstablishingConn ? NodeInteractionFlags.UnrequestingEdgeConn : NodeInteractionFlags.RequestingEdgeConn;
                }

            }
            else if( NodeUtils.SetTooltipForLastItem( string.Format( "[Left-click] to {0}\n[Right-click] to start connecting nodes", Packing == PackingStatus.PackingDone ? $"unpack {Pack.Count} child node(s)" : "pack up all child nodes" ) ) ) {
                tIsHovered = true;
            }
            ImGui.SetCursorScreenPos( tOriAnchor );
            // Draw
            pDrawList.AddCircleFilled(
                pNodeOSP - tSize, tSize.X * ( ( tIsHovered || pIsEstablishingConn || ( Packing != PackingStatus.None ) ) ? 2f : 1 ),
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency(
                    Packing == PackingStatus.None ? NodeUtils.Colors.NodeFg : NodeUtils.Colors.NodePack,
                    ( pIsActive || tIsHovered || pIsEstablishingConn ) ? 1f : 0.7f ) ) );
            pDrawList.AddCircleFilled( pNodeOSP - tSize, ( tSize.X * 0.7f ) * ( ( tIsHovered || pIsEstablishingConn || ( Packing != PackingStatus.None ) ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            pDrawList.AddCircleFilled( pNodeOSP - tSize, ( tSize.X * 0.5f ) * ( ( tIsHovered || pIsEstablishingConn ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorUnique, ( pIsActive || pIsEstablishingConn ) ? 0.55f : 0.25f ) ) );
            return tRes;
        }

        public abstract void Dispose();

        public enum PackingStatus {
            None = 0,       // synonamous with UnpackingDone
            PackingUnderway = 1,
            PackingDone = 2,
            UnpackingUnderway = 3
        }
    }
}
