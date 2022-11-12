using System.Numerics;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleSimpleColorCommand : ICommand {
        private readonly UISimpleColor Item;
        private readonly Vector4 PrevState;
        private readonly Vector4 State;

        public UiParticleSimpleColorCommand( UISimpleColor item, Vector4 prevState, Vector4 state ) {
            Item = item;
            PrevState = prevState;
            State = state;
        }

        public void Execute() { }

        public void Redo() => Item.SetColor( State );

        public void Undo() => Item.SetColor( PrevState );
    }
}
