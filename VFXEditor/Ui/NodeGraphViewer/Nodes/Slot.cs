using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Commands;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Ui.NodeGraphViewer.Nodes {
    public abstract class Slot {
        private const float SlotRadius = 5f;

        public readonly string Name;
        public readonly bool AcceptMultiple;

        private Slot SingleConnection;
        private readonly List<Slot> MultiConnections = [];

        public Node Node { get; protected set; }
        public int Index { get; protected set; }
        public bool IsInput { get; protected set; }

        public Slot( string name, bool acceptMultiple ) {
            Name = name;
            AcceptMultiple = acceptMultiple;
        }

        public void Setup( Node node, int index, bool isInput ) {
            Node = node;
            Index = index;
            IsInput = isInput;
        }

        public void Connect( Slot slot ) {
            if( AcceptMultiple ) {
                if( !MultiConnections.Contains( slot ) ) MultiConnections.Add( slot );
                return;
            }
            SingleConnection = slot;
        }

        public void Unconnect( Slot slot ) {
            if( AcceptMultiple ) MultiConnections.Remove( slot );
            else if( SingleConnection == slot ) SingleConnection = null;
        }

        public void Unconnect( Node node ) {
            if( AcceptMultiple ) MultiConnections.RemoveAll( x => x.Node == node );
            else if( SingleConnection?.Node == node ) SingleConnection = null;
        }

        public List<Slot> GetConnections() => AcceptMultiple ? MultiConnections : ( SingleConnection == null ? [] : [SingleConnection] );

        public void SetConnections( List<Slot> slots ) {
            if( AcceptMultiple ) {
                MultiConnections.Clear();
                MultiConnections.AddRange( slots );
                return;
            }
            SingleConnection = slots.Count == 0 ? null : slots[0];
        }

        public bool IsConnectedTo( Node node ) => GetConnections().Any( x => x.Node == node );

        public bool IsUnconnected() => GetConnections().Count == 0;

        public void Draw( ImDrawListPtr drawList, Vector2 position, float scaling, bool selected, Slot pending, out Slot _pending ) {
            var slotActive = pending == this;
            _pending = pending;
            using var _ = ImRaii.PushId( IsInput ? $"Input{Index}" : $"Output{Index}" );

            Vector2 size = new( SlotRadius, SlotRadius );
            var hovered = false;
            var cursor = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos( position - ( size * 3f ) / 2f );
            if( ImGui.InvisibleButton( "##Button", size * 3f ) ) {
                if( pending == null ) _pending = this;
                else {
                    if( pending.Node != Node && pending.IsInput != IsInput ) { // TODO: check cycle
                        if( IsInput ) CommandManager.Add( new NodeSlotCommand( this, pending, true ) );
                        else CommandManager.Add( new NodeSlotCommand( pending, this, true ) );
                    }
                    _pending = null;
                }

            }
            else if( ImGui.IsItemHovered() ) hovered = true;

            ImGui.SetCursorScreenPos( cursor );

            NodeUtils.PushFontScale( scaling );
            var textSize = ImGui.CalcTextSize( Name );
            drawList.AddText(
                position + new Vector2( IsInput ? 10f : ( -10f - textSize.X ), -textSize.Y / 2f ),
                IsUnconnected() && IsInput ? ImGui.GetColorU32( ImGuiCol.TextDisabled ) : ImGui.GetColorU32( ImGuiCol.Text ), Name );
            NodeUtils.PopFontScale();

            drawList.AddCircleFilled(
                position, size.X * ( ( hovered || slotActive ) ? 2f : 1 ),
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency(
                    NodeUtils.Colors.NodeFg,
                    ( selected || hovered || slotActive ) ? 1f : 0.7f ) ) );
            drawList.AddCircleFilled( position, ( size.X * 0.7f ) * ( ( hovered || slotActive ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( Node.Style.ColorBg ) );
            drawList.AddCircleFilled( position, ( size.X * 0.5f ) * ( ( hovered || slotActive ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Node.Style.ColorUnique, ( selected || slotActive ) ? 0.55f : 0.25f ) ) );
        }

        public abstract void DrawPopup( Slot target );

        public Vector2 GetSlotPosition( Vector2 nodePosition, float scaling ) => nodePosition + new Vector2(
            IsInput ? 0 : Node.Style.GetHandleSizeScaled( scaling ).X,
            ( Node.SlotSpacing * ( IsInput ? Index + Node.GetOutputCount() : Index ) + Node.Style.GetHandleSize().Y + ( Node.SlotSpacing / 2f ) ) * scaling );
    }
}
