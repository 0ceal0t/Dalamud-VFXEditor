using System;
using System.Numerics;
using VfxEditor;

namespace VfxEditor.Parsing {
    public class ParsedIntColorCommand : ICommand {
        private readonly ParsedIntColor Item;
        private readonly Vector4 State;
        private readonly Vector4 PrevState;

        public ParsedIntColorCommand( ParsedIntColor item, Vector4 prevState ) {
            Item = item;
            State = item.Value;
            PrevState = prevState;
        }

        public void Execute() { } // Color is already changed

        public void Redo() {
            Item.Value = State;
        }

        public void Undo() {
            Item.Value = PrevState;
        }
    }
}
