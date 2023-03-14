using System;
using VfxEditor;

namespace VfxEditor.Parsing {
    public class ParsedIntCommand : ICommand {
        private readonly ParsedInt Item;
        private readonly int State;
        private int PrevState;

        public ParsedIntCommand( ParsedInt item, int state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            PrevState = Item.Value;
            Item.Value = State;
        }

        public void Redo() {
            Item.Value = State;
        }

        public void Undo() {
            Item.Value = PrevState;
        }
    }
}
