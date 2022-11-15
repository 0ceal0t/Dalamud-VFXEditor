namespace VfxEditor.AvfxFormat2 {
    public class AvfxBinderDataExtraCommand : ICommand {
        private readonly AvfxBinder Item;
        private readonly AvfxData OldData;
        private AvfxData NewData;

        public AvfxBinderDataExtraCommand( AvfxBinder item ) {
            Item = item;
            OldData = item.Data;
        }

        public void Execute() {
            OldData?.Disable();
            Item.SetData( Item.BinderVariety.GetValue() );
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
