using System;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiFloatCommand : ICommand {
        private readonly AVFXFloat Item;
        private readonly float State;
        private readonly float PrevState;

        public UiFloatCommand( AVFXFloat item, float state ) {
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
