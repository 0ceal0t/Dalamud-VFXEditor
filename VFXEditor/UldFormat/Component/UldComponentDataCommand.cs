namespace VfxEditor.UldFormat.Component {
    public class UldComponentDataCommand : ICommand {
        private readonly UldComponent Item;
        private UldComponentData OldData;
        private UldComponentData NewData;

        public UldComponentDataCommand( UldComponent item ) {
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
