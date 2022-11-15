using System;
using System.Collections.Generic;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class UiAssignableCommandToggle<T> : ICommand where T : AvfxBase {
        private readonly List<T> Item;
        private readonly bool State;

        public UiAssignableCommandToggle( List<T> item, bool state ) {
            Item = item;
            State = state;
        }

        public void Execute() => Item.ForEach( x => x.SetAssigned( State ) );

        public void Redo() => Item.ForEach( x => x.SetAssigned( State ) );

        public void Undo() => Item.ForEach( x => x.SetAssigned( !State ) );
    }
}
