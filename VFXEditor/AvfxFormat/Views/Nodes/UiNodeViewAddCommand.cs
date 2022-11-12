using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiNodeViewAddCommand<T> : ICommand where T : UiNode {
        private readonly NodeRemover<T> Remover;

        public UiNodeViewAddCommand( IUiNodeView<T> view, UiNodeGroup<T> group, T item ) {
            Remover = new( view, group, item );
            // Already added
        }

        public void Execute() {
            // Do nothing
        }

        public void Redo() => Remover.Add();

        public void Undo() => Remover.Remove();
    }
}
