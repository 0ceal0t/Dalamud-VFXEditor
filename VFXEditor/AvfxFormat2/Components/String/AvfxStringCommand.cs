using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxStringCommand : ICommand {
        private readonly AvfxString Item;
        private readonly bool Unassign;
        private readonly bool PrevAssign;
        private readonly string State;
        private readonly string PrevState;

        public AvfxStringCommand( AvfxString item, string state, bool unassign ) {
            Item = item;
            Unassign = unassign;
            PrevAssign = item.IsAssigned();
            State = state;
            PrevState = item.GetValue();
        }

        public void Execute() {
            Item.SetValue( State );
            if( Unassign ) Item.SetAssigned( false );
        }

        public void Redo() {
            Item.SetValue( State );
            if( Unassign ) Item.SetAssigned( false );
        }

        public void Undo() {
            Item.SetValue( PrevState );
            Item.SetAssigned( PrevAssign );
        }
    }
}
