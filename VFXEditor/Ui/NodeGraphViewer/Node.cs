using ImGuiNET;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer {
    public abstract class Node {
        public static int NODE_ID = 0;

        public static readonly Vector2 NodeInsidePadding = new( 3.5f, 3.5f );
        public static readonly Vector2 MinHandleSize = new( 50, 20 );
        public static readonly float HandleButtonBoxItemWidth = 20;

        protected bool NeedReinit = false;
        public bool IsMarkedDeleted = false;

        public readonly int Id = NODE_ID++;

        public string Header { get; protected set; }

        public NodeStyle Style = new( Vector2.Zero, Vector2.Zero );
        protected virtual Vector2 RecommendedInitSize { get; } = new( 100, 200 );
        protected bool IsMinimized = false;
        public bool IsBusy = false;
        protected Vector2? LastUnminimizedSize = null;

        public Node( string header ) {
            Header = header;
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
                Style.SetSize( ImGui.CalcTextSize( Header ) + NodeInsidePadding * 2 );
        }

        public void Minimize() => IsMinimized = true;

        public void Unminimize() => IsMinimized = false;

        public NodeInteractionFlags Draw(
            Vector2 nodePos,
            float pCanvasScaling,
            bool pIsActive,
            InputPayload pInputPayload,
            bool pIsEstablishingConn = false ) {
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
            var tEnd = nodePos + tNodeSize;
            var tRes = NodeInteractionFlags.None;

            // resize grip
            if( !IsMinimized ) {
                Vector2 tGripSize = new( 10, 10 );
                tGripSize *= pCanvasScaling * 0.8f;     // making this scale less
                ImGui.SetCursorScreenPos( tEnd - tGripSize * ( pIsActive ? 0.425f : 0.57f ) );
                ImGui.PushStyleColor( ImGuiCol.Button, ImGui.ColorConvertFloat4ToU32( Style.ColorFg ) );
                ImGui.Button( $"##{Id}", tGripSize );
                if( ImGui.IsItemHovered() ) { ImGui.SetMouseCursor( ImGuiMouseCursor.ResizeNWSE ); }
                if( ImGui.IsItemActive() ) {
                    IsBusy = true;
                    Style.SetSizeScaled( pInputPayload.mMousePos - nodePos, pCanvasScaling );
                }
                if( IsBusy && !pInputPayload.mIsMouseLmbDown ) { IsBusy = false; }
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
                $"{Id}",
                tNodeSize,
                border: true,
                ImGuiWindowFlags.ChildWindow
                );
            // backdrop (leave this here so the backgrop can overwrite the child's bg)
            if( !IsMinimized ) drawList.AddRectFilled( nodePos, tEnd, ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            tRes |= DrawHandle( nodePos, pCanvasScaling, drawList, pIsActive );
            ImGui.SetCursorScreenPos( new Vector2( nodePos.X + 2 * pCanvasScaling, ImGui.GetCursorScreenPos().Y + 5 * pCanvasScaling ) );
            if( !IsMinimized ) tRes |= DrawBody( nodePos, pCanvasScaling );
            ImGui.EndChild();

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
            NodeUtils.PopFontScale();

            ImGui.EndChild();

            tRes |= DrawConnection( drawList, nodePos, pIsActive, establishingConnection: pIsEstablishingConn );

            return tRes;
        }

        protected virtual NodeInteractionFlags DrawHandle( Vector2 pNodeOSP, float pCanvasScaling, ImDrawListPtr pDrawList, bool pIsActive ) {
            var tHandleSize = Style.GetHandleSizeScaled( pCanvasScaling );
            pDrawList.AddRectFilled(
                pNodeOSP,
                pNodeOSP + tHandleSize,
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorUnique, pIsActive ? 0.45f : 0.15f ) ) );

            ImGui.TextColored( NodeUtils.Colors.NodeText, Header );

            // ButtonBox
            ImGui.SameLine();
            NodeUtils.AlignRight( HandleButtonBoxItemWidth * 2 * pCanvasScaling, pConsiderImguiPaddings: false );

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

        public NodeInteractionFlags DrawConnection( ImDrawListPtr drawList, Vector2 nodePosition, bool active, bool establishingConnection = false ) {
            var position = nodePosition;
            var tRes = NodeInteractionFlags.None;
            Vector2 tSize = new( 5f, 5f );
            var tIsHovered = false;
            var cursor = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos( position - ( tSize * 3f ) / 2f );
            if( ImGui.InvisibleButton( $"eb{Id}", tSize * 3f, ImGuiButtonFlags.MouseButtonRight | ImGuiButtonFlags.MouseButtonLeft ) ) {
                if( ImGui.GetIO().MouseReleased[0] ) {
                    tRes |= establishingConnection ? NodeInteractionFlags.UnrequestingEdgeConn : NodeInteractionFlags.RequestingEdgeConn;
                }

            }
            else if( ImGui.IsItemHovered() ) {
                tIsHovered = true;
            }

            ImGui.SetCursorScreenPos( cursor );
            drawList.AddCircleFilled(
                position, tSize.X * ( ( tIsHovered || establishingConnection ) ? 2f : 1 ),
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency(
                    NodeUtils.Colors.NodeFg,
                    ( active || tIsHovered || establishingConnection ) ? 1f : 0.7f ) ) );
            drawList.AddCircleFilled( position, ( tSize.X * 0.7f ) * ( ( tIsHovered || establishingConnection ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            drawList.AddCircleFilled( position, ( tSize.X * 0.5f ) * ( ( tIsHovered || establishingConnection ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorUnique, ( active || establishingConnection ) ? 0.55f : 0.25f ) ) );
            return tRes;
        }

        public Vector2 GetInputPosition( Vector2 nodePosition ) => nodePosition + new Vector2( -5, 10 );

        public abstract void Dispose();
    }
}
