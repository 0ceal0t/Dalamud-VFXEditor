namespace VfxEditor.AvfxFormat {
    public class UiNodeSelectListRemoveCommand<T> : ICommand where T : AvfxNode {
        private readonly UiNodeSelectList<T> Item;
        private readonly int Idx;
        private readonly T State;

        public UiNodeSelectListRemoveCommand( UiNodeSelectList<T> item, int idx ) {
            Item = item;
            Idx = idx;
            State = item.Selected[idx];
        }

        public void Execute() {
            Item.UnlinkParentChild( State );
            Item.Selected.RemoveAt( Idx );
        }

        public void Redo() => Execute();

        public void Undo() {
            Item.Selected.Insert( Idx, State );
            Item.LinkParentChild( State );
        }
    }
}
