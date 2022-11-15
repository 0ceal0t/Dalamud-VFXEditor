using System;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataExtraCommand : ICommand {
        private readonly AvfxParticle Item;
        private readonly AvfxData OldData;
        private AvfxData NewData;

        public AvfxParticleDataExtraCommand( AvfxParticle item ) {
            Item = item;
            OldData = item.Data;
        }

        public void Execute() {
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
