using System;
using System.Numerics;
using VfxEditor;

namespace VfxEditor.Parsing {
    public class ParsedStringCommand : ICommand {
        private readonly ParsedString Item;
        private readonly string State;
        private readonly string PrevState;

        public ParsedStringCommand( ParsedString item, string state, string prevState ) {
            Item = item;
            State = state;
            PrevState = prevState;
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