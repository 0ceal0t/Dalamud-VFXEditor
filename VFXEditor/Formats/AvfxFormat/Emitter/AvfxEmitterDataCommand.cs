namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterDataCommand : ICommand {
        private readonly AvfxEmitter Item;
        private AvfxData OldData;
        private AvfxData NewData;

        public AvfxEmitterDataCommand( AvfxEmitter item ) {
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
