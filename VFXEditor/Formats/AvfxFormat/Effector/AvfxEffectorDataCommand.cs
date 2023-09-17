namespace VfxEditor.AvfxFormat {
    public class AvfxEffectorDataCommand : ICommand {
        private readonly AvfxEffector Item;
        private AvfxData OldData;
        private AvfxData NewData;

        public AvfxEffectorDataCommand( AvfxEffector item ) {
            Item = item;
        }

        public void Execute() {
            OldData = Item.Data;
            OldData?.Disable();
            Item.SetData( Item.EffectorVariety.Value );
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
