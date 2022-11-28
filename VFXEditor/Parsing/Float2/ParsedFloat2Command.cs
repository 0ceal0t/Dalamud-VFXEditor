using System;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat2Command : ICommand {
        private readonly ParsedFloat2 Item;
        private readonly Vector2 State;
        private readonly Vector2 PrevState;

        public ParsedFloat2Command( ParsedFloat2 item, Vector2 state ) {
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
