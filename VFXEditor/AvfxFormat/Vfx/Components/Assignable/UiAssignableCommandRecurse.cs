using ImGuiNET;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiAssignableCommandRecurse : ICommand {
        private readonly AVFXBase Item;
        private readonly bool State;

        public UiAssignableCommandRecurse( AVFXBase item, bool state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            SetState( State );
        }

        public void Redo() {
            SetState( State );
        }

        public void Undo() {
            SetState( !State );
        }

        private void SetState( bool state ) {
            if( state ) AVFXBase.RecurseAssigned( Item, state ); // true
            else Item.SetAssigned( state ); // false
        }
    }
}
