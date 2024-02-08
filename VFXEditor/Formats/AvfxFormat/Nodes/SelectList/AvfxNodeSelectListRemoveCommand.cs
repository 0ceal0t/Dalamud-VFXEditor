namespace VfxEditor.AvfxFormat {
    public class AvfxNodeSelectListRemoveCommand<T> : ICommand where T : AvfxNode {
        private readonly AvfxNodeSelectList<T> Item;
        private readonly int Idx;
        private readonly T PrevState;

        public AvfxNodeSelectListRemoveCommand( AvfxNodeSelectList<T> item, int idx ) {
            Item = item;
            Idx = idx;
            PrevState = Item.Selected[Idx];

            Item.UnlinkParentChild( PrevState );
            Item.Selected.RemoveAt( Idx );
            Item.UpdateLiteral();
        }

        public void Redo() {
            Item.UnlinkParentChild( PrevState );
            Item.Selected.RemoveAt( Idx );
            Item.UpdateLiteral();
        }

        public void Undo() {
            Item.Selected.Insert( Idx, PrevState );
            Item.LinkParentChild( PrevState );
            Item.UpdateLiteral();
        }
    }
}
