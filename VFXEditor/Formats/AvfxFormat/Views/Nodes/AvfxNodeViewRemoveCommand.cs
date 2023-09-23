namespace VfxEditor.AvfxFormat {
    public class AvfxNodeViewRemoveCommand<T> : ICommand where T : AvfxNode {
        private readonly AvfxNodeRemover<T> Remover;

        public AvfxNodeViewRemoveCommand( IUiNodeView<T> view, NodeGroup<T> group, T item ) {
            Remover = new( view, group, item );
        }

        public void Execute() {
            Remover.Execute();
            Remover.Remove();
        }

        public void Redo() => Remover.Remove();

        public void Undo() => Remover.Add();
    }
}
