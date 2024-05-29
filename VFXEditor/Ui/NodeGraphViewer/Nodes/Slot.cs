using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Commands;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Ui.NodeGraphViewer.Nodes {
    public class Slot {
        public readonly Node Node;
        public readonly bool IsInput;
        public readonly string Name;
        public Slot Connected { get; protected set; }
        public int Index { get; protected set; }

        private const float SlotRadius = 5f;

        public bool IsEmpty => Connected == null;

        public Slot( Node node, string name, bool isInput ) {
            Node = node;
            IsInput = isInput;
            Name = name;
        }

        public void SetIndex( int index ) {
            Index = index;
        }

        public void ConnectTo( Slot slot ) {
            Connected = slot;
        }

        public void Draw( ImDrawListPtr drawList, Vector2 position, bool selected, Slot connection, out Slot _connection ) {
            var slotActive = connection == this;
            _connection = connection;
            using var _ = ImRaii.PushId( IsInput ? $"Input{Index}" : $"Output{Index}" );

            Vector2 size = new( SlotRadius, SlotRadius );
            var hovered = false;
            var cursor = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos( position - ( size * 3f ) / 2f );
            if( ImGui.InvisibleButton( $"##Button", size * 3f ) ) {
                if( connection == null ) _connection = this;
                else {
                    if( connection.Node != Node && connection.IsInput != IsInput ) { // TODO: check cycle
                        if( IsInput ) CommandManager.Add( new NodeSlotCommand( this, connection ) );
                        else CommandManager.Add( new NodeSlotCommand( connection, this ) );
                    }
                    _connection = null;
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

        public Vector2 GetSlotPosition( Vector2 nodePosition, float scaling ) => nodePosition + new Vector2(
            IsInput ? 0 : Node.Style.GetHandleSizeScaled( scaling ).X,
            ( Node.SlotSpacing * Index + Node.Style.GetHandleSize().Y + ( Node.SlotSpacing / 2f ) ) * scaling );
    }
}
