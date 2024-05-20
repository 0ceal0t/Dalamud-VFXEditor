using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Ui.NodeGraphViewer.Nodes {
    public class Slot {
        public readonly Node Node;
        public readonly bool IsInput;
        public readonly string Name;
        public readonly int Index;
        public Node Connected { get; protected set; }

        private const float SlotRadius = 5f;

        public bool IsEmpty => Connected == null;

        public Slot( Node node, string name, int index, bool isInput = true ) {
            Node = node;
            Index = index;
            IsInput = isInput;
            Name = name;
        }

        public void Clear() {
            Connected = null;
        }

        public void ConnectTo( Node node ) {
            Connected = node;
        }

        public void Draw( ImDrawListPtr drawList, Vector2 position, bool selected, Slot connection, out Slot _connection ) {
            var slotActive = connection == this;
            _connection = connection;
            using var _ = ImRaii.PushId( IsInput ? $"Input{Index}" : "Output" );

            Vector2 size = new( SlotRadius, SlotRadius );
            var hovered = false;
            var cursor = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos( position - ( size * 3f ) / 2f );
            if( ImGui.InvisibleButton( $"##Button", size * 3f, ImGuiButtonFlags.MouseButtonRight | ImGuiButtonFlags.MouseButtonLeft ) ) {
                if( ImGui.GetIO().MouseReleased[0] ) {
                    if( connection == null ) {
                        _connection = this;
                    }
                    else {
                        if( connection.Node != Node && connection.IsInput != IsInput ) { // TODO: check cycle
                            if( IsInput ) ConnectTo( connection.Node );
                            else connection.ConnectTo( Node );
                        }
                        _connection = null;
                    }
                }

            }
            else if( ImGui.IsItemHovered() ) hovered = true;

            ImGui.SetCursorScreenPos( cursor );
            drawList.AddCircleFilled(
                position, size.X * ( ( hovered || slotActive ) ? 2f : 1 ),
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency(
                    NodeUtils.Colors.NodeFg,
                    ( selected || hovered || slotActive ) ? 1f : 0.7f ) ) );
            drawList.AddCircleFilled( position, ( size.X * 0.7f ) * ( ( hovered || slotActive ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( Node.Style.ColorBg ) );
            drawList.AddCircleFilled( position, ( size.X * 0.5f ) * ( ( hovered || slotActive ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Node.Style.ColorUnique, ( selected || slotActive ) ? 0.55f : 0.25f ) ) );
        }
    }
}
