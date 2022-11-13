using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxFloatCommand : ICommand {
        private readonly AvfxFloat Item;
        private readonly float State;
        private readonly float PrevState;

        public AvfxFloatCommand( AvfxFloat item, float state ) {
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
