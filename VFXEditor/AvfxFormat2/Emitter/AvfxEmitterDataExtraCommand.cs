using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitterDataExtraCommand : ICommand {
        private readonly AvfxEmitter Item;
        private readonly AvfxData OldData;
        private AvfxData NewData;

        public AvfxEmitterDataExtraCommand( AvfxEmitter item ) {
            Item = item;
            OldData = item.Data;
        }

        public void Execute() {
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
