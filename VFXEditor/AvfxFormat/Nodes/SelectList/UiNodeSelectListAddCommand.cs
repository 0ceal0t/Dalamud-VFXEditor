namespace VfxEditor.AvfxFormat {
    public class UiNodeSelectListAddCommand<T> : ICommand where T : AvfxNode {
        private readonly UiNodeSelectList<T> Item;

        public UiNodeSelectListAddCommand( UiNodeSelectList<T> item ) {
            Item = item;
        }

        public void Execute() {
            var item = Item.Group.Items[0];
            Item.Selected.Add( item );
            Item.LinkParentChild( item );
        }

        public void Redo() => Execute();

        public void Undo() {
            var idx = Item.Selected.Count - 1;
            Item.UnlinkParentChild( Item.Selected[idx] );
            Item.Selected.RemoveAt( idx );
        }
    }
}
