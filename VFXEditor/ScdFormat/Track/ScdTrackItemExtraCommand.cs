using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.ScdFormat {
    public class ScdTrackItemExtraCommand : ICommand {
        private readonly ScdTrackItem Item;
        private ScdTrackData OldData;
        private ScdTrackData NewData;

        public ScdTrackItemExtraCommand( ScdTrackItem item ) {
            Item = item;
        }

        public void Execute() {
            OldData = Item.Data;
            Item.UpdateData();
            NewData = Item.Data;
        }

        public void Redo() {
            Item.Data = NewData;
        }

        public void Undo() {
            Item.Data = OldData;
        }
    }
}
