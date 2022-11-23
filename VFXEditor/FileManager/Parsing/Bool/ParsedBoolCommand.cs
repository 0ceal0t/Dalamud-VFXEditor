using System;

namespace VfxEditor.Parsing {
    public class ParsedBoolCommand : ICommand {
        private readonly ParsedBool Item;
        private readonly bool State;
        private readonly bool? PrevState;

        public ParsedBoolCommand( ParsedBool item, bool state ) {
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
