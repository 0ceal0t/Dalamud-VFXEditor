namespace VfxEditor.AvfxFormat {
    public class UiNodeSelectListAddCommand<T> : ICommand where T : AvfxNode {
        private readonly UiNodeSelectList<T> Item;

        public UiNodeSelectListAddCommand( UiNodeSelectList<T> item ) {
            Item = item;
        }

        public void Execute() {
            Item.Selected.Add( null );
            Item.UpdateLiteral();
        }

        public void Redo() => Execute();

        public void Undo() {
            Item.Selected.RemoveAt( Item.Selected.Count - 1 );
            Item.UpdateLiteral();
        }
    }
}
