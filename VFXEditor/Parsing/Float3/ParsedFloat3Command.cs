using System;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat3Command : ICommand {
        private readonly ParsedFloat3 Item;
        private readonly Vector3 State;
        private Vector3 PrevState;

        public ParsedFloat3Command( ParsedFloat3 item, Vector3 state ) {
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
