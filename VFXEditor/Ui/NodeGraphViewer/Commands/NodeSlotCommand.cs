using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Ui.NodeGraphViewer.Commands {
    public class NodeSlotCommand : ICommand {
        public readonly Slot Slot;
        public readonly Node State;
        public readonly Node PrevState;

        public NodeSlotCommand( Slot slot, Node state ) {
            Slot = slot;
            State = state;
            PrevState = slot.Connected;
            Slot.ConnectTo( State );
        }

        public void Redo() => Slot.ConnectTo( State );

        public void Undo() => Slot.ConnectTo( PrevState );
    }
}
