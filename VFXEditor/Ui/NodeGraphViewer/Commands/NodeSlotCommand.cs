using System.Collections.Generic;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Ui.NodeGraphViewer.Commands {
    public class NodeSlotCommand : ICommand {
        public readonly Slot Slot;
        public readonly List<Slot> State;
        public readonly List<Slot> PrevState;

        public NodeSlotCommand( Slot slot, Slot target, bool connect ) {
            Slot = slot;
            PrevState = slot.GetConnections();
            if( connect ) Slot.Connect( target );
            else Slot.Unconnect( target );
            State = slot.GetConnections();
        }

        public void Redo() => Slot.SetConnections( State );

        public void Undo() => Slot.SetConnections( PrevState );
    }
}
