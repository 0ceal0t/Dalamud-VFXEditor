using System;

namespace VfxEditor.Parsing {
    public class ParsedFloatCommand : ICommand {
        private readonly ParsedFloat Item;
        private readonly float State;
        private float PrevState;

        public ParsedFloatCommand( ParsedFloat item, float state ) {
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
