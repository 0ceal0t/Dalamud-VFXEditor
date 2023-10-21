namespace VfxEditor.Parsing {
    public class ParsedSimpleCommand<S> : ICommand {
        private readonly ParsedSimpleBase<S> Item;
        private readonly ICommand ExtraCommand;
        private readonly S State;
        private S PrevState;

        private readonly bool PrevStateSet = false;

        public ParsedSimpleCommand( ParsedSimpleBase<S> item, S prevState, S state, ICommand extraCommand = null ) : this( item, state, extraCommand ) {
            PrevState = prevState;
            PrevStateSet = true;
        }

        public ParsedSimpleCommand( ParsedSimpleBase<S> item, S state, ICommand extraCommand = null ) {
            Item = item;
            State = state;
            ExtraCommand = extraCommand;
        }

        public void Execute() {
            if( !PrevStateSet ) PrevState = Item.Value;
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
