namespace VfxEditor.AvfxFormat {
    public class UiNodeSelectCommand<T> : ICommand where T : AvfxNode {
        private readonly UiNodeSelect<T> Item;
        private readonly T State;
        private T PrevState;

        public UiNodeSelectCommand( UiNodeSelect<T> item, T state ) {
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
