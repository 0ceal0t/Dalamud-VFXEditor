using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Parsing {
    public class ParsedFlagCommand<T> : ICommand where T : Enum {
        private readonly ParsedFlag<T> Item;
        private readonly T State;
        private T PrevState;
        private readonly ICommand ExtraCommand;

        public ParsedFlagCommand( ParsedFlag<T> item, T state, ICommand extraCommand ) {
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
