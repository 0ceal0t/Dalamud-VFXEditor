using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class AvfxAssignCommandToggle<T> : ICommand where T : AvfxBase {
        private readonly List<T> Item;
        private readonly bool Assigned;

        public AvfxAssignCommandToggle( List<T> item, bool assigned ) {
            Item = item;
            Assigned = assigned;
        }

        public void Execute() => Item.ForEach( x => x.SetAssigned( Assigned ) );

        public void Redo() => Item.ForEach( x => x.SetAssigned( Assigned ) );

        public void Undo() => Item.ForEach( x => x.SetAssigned( !Assigned ) );
    }
}
