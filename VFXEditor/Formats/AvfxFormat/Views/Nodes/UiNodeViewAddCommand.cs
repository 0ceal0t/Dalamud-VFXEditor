using VfxEditor.Ui.Nodes;

namespace VfxEditor.AvfxFormat {
    public class UiNodeViewAddCommand<T> : ICommand where T : AvfxNode {
        private readonly UiNodeRemover<T> Remover;

        public UiNodeViewAddCommand( IUiNodeView<T> view, NodeGroup<T> group, T item ) {
            Remover = new( view, group, item ); // Already added, just need to prep the remover
        }

        public void Execute() => Remover.Execute();

        public void Redo() => Remover.Add();

        public void Undo() => Remover.Remove();
    }
}
