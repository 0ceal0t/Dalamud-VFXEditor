using System;
using System.Numerics;
using VfxEditor;

namespace VfxEditor.Parsing {
    public class ParsedIntColorCommand : ICommand {
        private readonly ParsedIntColor Item;
        private readonly Vector4 State;
        private Vector4 PrevState;

        public ParsedIntColorCommand( ParsedIntColor item, Vector4 state, Vector4 prevState ) {
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
