using Dalamud.Logging;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiCheckboxCommand : ICommand {
        private readonly AVFXBool Item;
        private readonly bool State;
        private readonly bool? PrevState;

        public UiCheckboxCommand( AVFXBool item, bool state ) {
            Item = item;
            State = state;
            PrevState = item.GetValue();
        }

        public void Execute() {
            Item.SetValue( State );
        }

        public void Redo() {
            Item.SetValue( State );
        }

        public void Undo() {
            Item.SetValue( PrevState );
        }
    }
}
