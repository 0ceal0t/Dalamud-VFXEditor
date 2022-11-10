using System;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiIntListCommand : ICommand {
        private readonly AVFXIntList Item;
        private readonly int State;
        private readonly int PrevState;

        public UiIntListCommand( AVFXIntList item, int state ) {
            Item = item;
            State = state;
            PrevState = item.GetValue()[0];
        }

        public void Execute() {
            Item.GetValue()[0] = State;
        }

        public void Redo() {
            Item.GetValue()[0] = State;
        }

        public void Undo() {
            Item.GetValue()[0] = PrevState;
        }
    }
}
