using ImGuiNET;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiAssignableCommand : ICommand {
        private readonly AVFXBase Item;
        private readonly bool State;

        public UiAssignableCommand( AVFXBase item, bool state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            Item.SetAssigned( State );
        }

        public void Redo() {
            Item.SetAssigned( State );
        }

        public void Undo() {
            Item.SetAssigned( !State );
        }
    }
}
