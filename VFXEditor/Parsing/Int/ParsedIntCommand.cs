using System;
using VfxEditor;

namespace VfxEditor.Parsing {
    public class ParsedIntCommand : ICommand {
        private readonly ParsedInt Item;
        private readonly int State;
        private readonly int PrevState;

        public ParsedIntCommand( ParsedInt item, int state ) {
            Item = item;
            State = state;
            PrevState = item.Value;
        }

        public void Execute() {
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
