namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataCommand : ICommand {
        private readonly AvfxParticle Item;
        private AvfxData OldData;
        private AvfxData NewData;

        public AvfxParticleDataCommand( AvfxParticle item ) {
            Item = item;
        }

        public void Execute() {
            OldData = Item.Data;
            OldData?.Disable();
            Item.SetData( Item.ParticleVariety.GetValue() );
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
