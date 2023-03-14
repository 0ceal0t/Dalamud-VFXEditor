using System;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataExtraCommand : ICommand {
        private readonly AvfxParticle Item;
        private AvfxData OldData;
        private AvfxData NewData;

        public AvfxParticleDataExtraCommand( AvfxParticle item ) {
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
