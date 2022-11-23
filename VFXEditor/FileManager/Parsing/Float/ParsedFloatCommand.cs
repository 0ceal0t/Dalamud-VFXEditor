using System;

namespace VfxEditor.Parsing {
    public class ParsedFloatCommand : ICommand {
        private readonly ParsedFloat Item;
        private readonly float State;
        private readonly float PrevState;

        public ParsedFloatCommand( ParsedFloat item, float state ) {
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
