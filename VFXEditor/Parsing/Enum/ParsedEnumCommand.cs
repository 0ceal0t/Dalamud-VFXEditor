using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Parsing {
    public class ParsedEnumCommand<T> : ICommand where T : Enum {
        private readonly ParsedEnum<T> Item;
        private readonly T State;
        private readonly T PrevState;
        private readonly ICommand ExtraCommand;

        public ParsedEnumCommand( ParsedEnum<T> item, T state, ICommand extraCommand ) {
            Item = item;
            State = state;
            PrevState = item.Value;
            ExtraCommand = extraCommand;
        }

        public void Execute() {
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
