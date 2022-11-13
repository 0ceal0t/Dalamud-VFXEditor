using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class UiParticleSimpleFrameCommand : ICommand {
        private readonly UiSimpleColor Item;
        private readonly int PrevState;
        private readonly int State;

        public UiParticleSimpleFrameCommand( UiSimpleColor item, int state ) {
            Item = item;
            PrevState = Item.GetFrame();
            State = state;
        }

        public void Execute() => Item.SetFrame( State );

        public void Redo() => Item.SetFrame( State );

        public void Undo() => Item.SetFrame( PrevState );
    }
}
