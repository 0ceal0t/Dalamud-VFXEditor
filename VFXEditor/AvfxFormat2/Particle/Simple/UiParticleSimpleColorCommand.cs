using System.Numerics;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class UiParticleSimpleColorCommand : ICommand {
        private readonly UiSimpleColor Item;
        private readonly Vector4 PrevState;
        private readonly Vector4 State;

        public UiParticleSimpleColorCommand( UiSimpleColor item, Vector4 prevState, Vector4 state ) {
            Item = item;
            PrevState = prevState;
            State = state;
        }

        public void Execute() { }

        public void Redo() => Item.SetColor( State );

        public void Undo() => Item.SetColor( PrevState );
    }
}
