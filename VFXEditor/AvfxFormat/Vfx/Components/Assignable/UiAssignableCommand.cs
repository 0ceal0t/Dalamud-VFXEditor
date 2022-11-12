using ImGuiNET;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiAssignableCommand : ICommand {
        private readonly AVFXBase Item;
        private readonly bool State;
        private readonly bool PrevState;

        public UiAssignableCommand( AVFXBase item, bool state ) {
            Item = item;
            State = state;
            PrevState = item.IsAssigned();
        }

        public void Execute() {
            Item.SetAssigned( State );
        }

        public void Redo() {
            Item.SetAssigned( State );
        }

        public void Undo() {
            Item.SetAssigned( PrevState );
        }
    }
}
