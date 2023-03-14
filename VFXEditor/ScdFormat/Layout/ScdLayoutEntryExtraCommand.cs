namespace VfxEditor.ScdFormat {
    public class ScdLayoutEntryExtraCommand : ICommand {
        private readonly ScdLayoutEntry Item;
        private ScdLayoutData OldData;
        private ScdLayoutData NewData;

        public ScdLayoutEntryExtraCommand( ScdLayoutEntry item ) {
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
