using System;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiNodeViewRemoveCommand<T> : ICommand where T : UiNode {
        private readonly NodeRemover<T> Remover;

        public UiNodeViewRemoveCommand( IUiNodeView<T> view, UiNodeGroup<T> group, T item ) {
            Remover = new( view, group, item );
        }

        public void Execute() => Remover.Remove();

        public void Redo() => Remover.Remove();

        public void Undo() => Remover.Add();
    }
}
