namespace VfxEditor.AvfxFormat {
    public class AvfxNodeSelectCommand<T> : ICommand where T : AvfxNode {
        private readonly AvfxNodeSelect<T> Item;
        private readonly T State;
        private T PrevState;

        public AvfxNodeSelectCommand( AvfxNodeSelect<T> item, T state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            PrevState = Item.Selected;
            Item.Select( State );
        }

        public void Redo() => Item.Select( State );

        public void Undo() => Item.Select( PrevState );
    }
}
