namespace VfxEditor.AvfxFormat {
    public class UiNodeViewAddCommand<T> : ICommand where T : AvfxNode {
        private readonly UiNodeRemover<T> Remover;

        public UiNodeViewAddCommand( IUiNodeView<T> view, UiNodeGroup<T> group, T item ) {
            Remover = new( view, group, item ); // Already added, just need to prep the remover
        }

        public void Execute() { }

        public void Redo() => Remover.Add();

        public void Undo() => Remover.Remove();
    }
}
