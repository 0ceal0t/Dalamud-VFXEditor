using System;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat2Command : ICommand {
        private readonly ParsedFloat2 Item;
        private readonly Vector2 State;
        private Vector2 PrevState;

        public ParsedFloat2Command( ParsedFloat2 item, Vector2 state ) {
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
