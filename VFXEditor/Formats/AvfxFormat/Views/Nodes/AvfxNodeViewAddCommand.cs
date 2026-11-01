using VfxEditor.Data.Command;
using VfxEditor.Formats.AvfxFormat.Nodes;

namespace VfxEditor.Formats.AvfxFormat.Views.Nodes {
    public class AvfxNodeViewAddCommand<T> : ICommand where T : AvfxNode {
        private readonly AvfxNodeRemover<T> Remover;

        public AvfxNodeViewAddCommand( IUiNodeView<T> view, NodeGroup<T> group, T item ) {
            Remover = new( view, group, item ); // Already added, just need to prep the remover
        }

        public void Redo() => Remover.Add();

        public void Undo() => Remover.Remove();
    }
}
