namespace VfxEditor.ScdFormat {
    public class ScdTrackDataCommand : ICommand {
        private readonly ScdTrackItem Item;
        private ScdTrackData OldData;
        private ScdTrackData NewData;

        public ScdTrackDataCommand( ScdTrackItem item ) {
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
