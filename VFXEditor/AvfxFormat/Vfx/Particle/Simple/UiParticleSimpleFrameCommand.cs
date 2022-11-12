using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleSimpleFrameCommand : ICommand {
        private readonly UISimpleColor Item;
        private readonly int PrevState;
        private readonly int State;

        public UiParticleSimpleFrameCommand( UISimpleColor item, int state ) {
            Item = item;
            PrevState = Item.GetFrame();
            State = state;
        }

        public void Execute() => Item.SetFrame( State );

        public void Redo() => Item.SetFrame( State );

        public void Undo() => Item.SetFrame( PrevState );
    }
}
