namespace VfxEditor.AvfxFormat {
    public class AvfxNodeSelectListCommand<T> : ICommand where T : AvfxNode {
        private readonly AvfxNodeSelectList<T> Item;
        private readonly int Idx;
        private readonly T State;
        private T PrevState;

        public AvfxNodeSelectListCommand( AvfxNodeSelectList<T> item, T state, int idx ) {
            Item = item;
            State = state;
            Idx = idx;
        }

        public void Execute() {
            PrevState = Item.Selected[Idx];
            Item.Select( State, Idx );
        }

        public void Redo() {
            Item.Select( State, Idx );
        }

        public void Undo() {
            Item.Select( PrevState, Idx );
        }
    }
}