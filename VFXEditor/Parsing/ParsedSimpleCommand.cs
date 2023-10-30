using System;

namespace VfxEditor.Parsing {
    public class ParsedSimpleCommand<S> : ICommand {
        private readonly ParsedSimpleBase<S> Item;
        private readonly Action OnChangeAction;
        private readonly S State;
        private S PrevState;

        private readonly bool PrevStateSet = false;

        public ParsedSimpleCommand( ParsedSimpleBase<S> item, S prevState, S state, Action onChangeAction = null ) : this( item, state, onChangeAction ) {
            PrevState = prevState;
            PrevStateSet = true;
        }

        public ParsedSimpleCommand( ParsedSimpleBase<S> item, S state, Action onChangeAction = null ) {
            Item = item;
            State = state;
            OnChangeAction = onChangeAction;
        }

        public void Execute() {
            if( !PrevStateSet ) PrevState = Item.Value;
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
