using System;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxIntCommand : ICommand {
        private readonly AvfxInt Item;
        private readonly int State;
        private readonly int PrevState;

        public AvfxIntCommand( AvfxInt item, int state ) {
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
