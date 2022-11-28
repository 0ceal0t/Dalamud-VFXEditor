using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.ScdFormat {
    public class ScdLayoutEntryExtraCommand : ICommand {
        private readonly ScdLayoutEntry Item;
        private readonly ScdLayoutData OldData;
        private ScdLayoutData NewData;

        public ScdLayoutEntryExtraCommand( ScdLayoutEntry item ) {
            Item = item;
            OldData = item.Data;
        }

        public void Execute() {
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
