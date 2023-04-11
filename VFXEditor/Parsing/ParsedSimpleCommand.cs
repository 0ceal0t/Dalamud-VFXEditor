namespace VfxEditor.Parsing {
    public class ParsedSimpleCommand<S> : ICommand {
        private readonly ParsedSimpleBase<S> Item;
        private readonly S State;
        private S PrevState;

        private readonly bool PrevStateSet = false;

        public ParsedSimpleCommand( ParsedSimpleBase<S> item, S prevState, S state ) {
            Item = item;
            State = state;
            PrevState = prevState;
            PrevStateSet = true;
        }

        public ParsedSimpleCommand( ParsedSimpleBase<S> item, S state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            if( !PrevStateSet ) PrevState = Item.Value;
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
