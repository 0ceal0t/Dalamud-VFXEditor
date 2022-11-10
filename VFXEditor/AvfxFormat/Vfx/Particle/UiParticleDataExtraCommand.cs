using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataExtraCommand : ICommand {
        private readonly UiParticle Item;
        private readonly AVFXBase OldData;
        private readonly UiData OldUi;

        private AVFXBase NewData;
        private UiData NewUi;

        public UiParticleDataExtraCommand( UiParticle item ) {
            Item = item;
            OldUi = item.Data;
            OldData = item.Particle.Data;
        }

        public void Execute() {
            Item.Particle.SetType( Item.Particle.ParticleVariety.GetValue() );
            Item.UpdateDataType(); // already disables the old one

            NewData = Item.Particle.Data;
            NewUi = Item.Data;
        }

        public void Redo() {
            OldUi?.Disable();
            Item.Particle.Data = NewData;
            Item.Data = NewUi;
            NewUi?.Enable();
        }

        public void Undo() {
            NewUi?.Disable();
            Item.Particle.Data = OldData;
            Item.Data = OldUi;
            OldUi?.Enable();
        }
    }
}
