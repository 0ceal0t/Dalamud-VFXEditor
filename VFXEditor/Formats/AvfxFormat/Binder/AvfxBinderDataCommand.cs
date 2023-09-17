namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataCommand : ICommand {
        private readonly AvfxBinder Item;
        private AvfxData OldData;
        private AvfxData NewData;

        public AvfxBinderDataCommand( AvfxBinder item ) {
            Item = item;
        }

        public void Execute() {
            OldData = Item.Data;
            OldData?.Disable();
            Item.SetData( Item.BinderVariety.Value );
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
