using System;

namespace VfxEditor.Parsing {
    public class ParsedEnumCommand<T> : ICommand where T : Enum {
        private readonly ParsedEnum<T> Item;
        private readonly T State;
        private T PrevState;
        private readonly ICommand ExtraCommand;

        public ParsedEnumCommand( ParsedEnum<T> item, T state, ICommand extraCommand ) {
            Item = item;
            State = state;
            ExtraCommand = extraCommand;
        }

        public void Execute() {
            PrevState = Item.Value;
            Item.Value = State;
            ExtraCommand?.Execute();
        }

        public void Redo() {
            Item.Value = State;
            ExtraCommand?.Redo();
        }

        public void Undo() {
            Item.Value = PrevState;
            ExtraCommand?.Undo();
        }
    }
}
