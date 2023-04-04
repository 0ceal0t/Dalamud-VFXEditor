using System;
using VfxEditor;

namespace VfxEditor.Parsing {
    public class ParsedUIntCommand : ICommand {
        private readonly ParsedUInt Item;
        private readonly uint State;
        private uint PrevState;

        public ParsedUIntCommand( ParsedUInt item, uint state ) {
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
