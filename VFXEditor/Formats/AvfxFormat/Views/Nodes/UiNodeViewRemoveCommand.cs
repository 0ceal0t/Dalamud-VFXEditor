using VfxEditor.Ui.Nodes;

namespace VfxEditor.AvfxFormat {
    public class UiNodeViewRemoveCommand<T> : ICommand where T : AvfxNode {
        private readonly UiNodeRemover<T> Remover;

        public UiNodeViewRemoveCommand( IUiNodeView<T> view, NodeGroup<T> group, T item ) {
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
