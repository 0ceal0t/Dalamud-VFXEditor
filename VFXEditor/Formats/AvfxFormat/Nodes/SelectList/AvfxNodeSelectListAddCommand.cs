namespace VfxEditor.AvfxFormat {
    public class AvfxNodeSelectListAddCommand<T> : ICommand where T : AvfxNode {
        private readonly AvfxNodeSelectList<T> Item;

        public AvfxNodeSelectListAddCommand( AvfxNodeSelectList<T> item ) {
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
