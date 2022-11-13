using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
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
