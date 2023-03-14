using System;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat4Command : ICommand {
        private readonly ParsedFloat4 Item;
        private readonly Vector4 State;
        private Vector4 PrevState;

        public ParsedFloat4Command( ParsedFloat4 item, Vector4 state ) {
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
