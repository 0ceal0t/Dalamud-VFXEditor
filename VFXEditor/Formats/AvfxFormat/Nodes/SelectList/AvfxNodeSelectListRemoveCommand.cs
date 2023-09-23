namespace VfxEditor.AvfxFormat {
    public class AvfxNodeSelectListRemoveCommand<T> : ICommand where T : AvfxNode {
        private readonly AvfxNodeSelectList<T> Item;
        private readonly int Idx;
        private T PrevState;

        public AvfxNodeSelectListRemoveCommand( AvfxNodeSelectList<T> item, int idx ) {
            Item = item;
            Idx = idx;
        }

        public void Execute() {
            PrevState = Item.Selected[Idx];
            Item.UnlinkParentChild( PrevState );
            Item.Selected.RemoveAt( Idx );
            Item.UpdateLiteral();
        }

        public void Redo() => Execute();

        public void Undo() {
            Item.Selected.Insert( Idx, PrevState );
            Item.LinkParentChild( PrevState );
            Item.UpdateLiteral();
        }
    }
}
