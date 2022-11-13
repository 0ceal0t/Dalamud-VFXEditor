using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxBoolCommand : ICommand {
        private readonly AvfxBool Item;
        private readonly bool State;
        private readonly bool? PrevState;

        public AvfxBoolCommand( AvfxBool item, bool state ) {
            Item = item;
            State = state;
            PrevState = item.GetValue();
        }

        public void Execute() {
            Item.SetValue( State );
        }

        public void Redo() {
            Item.SetValue( State );
        }

        public void Undo() {
            Item.SetValue( PrevState );
        }
    }
}
