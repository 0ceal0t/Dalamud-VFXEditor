using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class UiNodeViewRemoveCommand<T> : ICommand where T : AvfxNode {
        private readonly UiNodeRemover<T> Remover;

        public UiNodeViewRemoveCommand( IUiNodeView<T> view, UiNodeGroup<T> group, T item ) {
            Remover = new( view, group, item );
        }

        public void Execute() => Remover.Remove();

        public void Redo() => Remover.Remove();

        public void Undo() => Remover.Add();
    }
}
