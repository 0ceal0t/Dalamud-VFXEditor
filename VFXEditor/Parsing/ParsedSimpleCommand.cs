using System;

namespace VfxEditor.Parsing {
    public class ParsedSimpleCommand<S> : ICommand {
        private readonly ParsedSimpleBase<S> Item;
        private readonly Action OnChangeAction;
        private readonly S State;
        private readonly S PrevState;

        public ParsedSimpleCommand( ParsedSimpleBase<S> item, S state, Action onChangeAction = null ) : this( item, item.Value, state, onChangeAction ) { }

        public ParsedSimpleCommand( ParsedSimpleBase<S> item, S prevState, S state, Action onChangeAction = null ) {
            Item = item;
            State = state;
            PrevState = prevState;
            OnChangeAction = onChangeAction;

            Item.Value = State;
            OnChangeAction?.Invoke();
        }

        public void Redo() {
            Item.Value = State;
            OnChangeAction?.Invoke();
        }

        public void Undo() {
            Item.Value = PrevState;
            OnChangeAction?.Invoke();
        }
    }
}
