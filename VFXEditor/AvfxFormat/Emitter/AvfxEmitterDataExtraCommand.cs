using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterDataExtraCommand : ICommand {
        private readonly AvfxEmitter Item;
        private AvfxData OldData;
        private AvfxData NewData;

        public AvfxEmitterDataExtraCommand( AvfxEmitter item ) {
            Item = item;
        }

        public void Execute() {
            OldData = Item.Data;
            OldData?.Disable();
            Item.SetData( Item.EmitterVariety.GetValue() );
            NewData = Item.Data;
        }

        public void Redo() {
            OldData?.Disable();
            Item.Data = NewData;
            NewData?.Enable();
        }

        public void Undo() {
            NewData?.Disable();
            Item.Data = OldData;
            OldData?.Enable();
        }
    }
}
