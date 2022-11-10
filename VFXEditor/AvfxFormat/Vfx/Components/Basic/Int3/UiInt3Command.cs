using System;
using System.Numerics;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiInt3Command : ICommand {
        private readonly AVFXInt Item1;
        private readonly AVFXInt Item2;
        private readonly AVFXInt Item3;

        private readonly Vector3 State;
        private readonly Vector3 PrevState;

        public UiInt3Command( AVFXInt item1, AVFXInt item2, AVFXInt item3, Vector3 state ) {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            State = state;
            PrevState = new Vector3( item1.GetValue(), item2.GetValue(), item3.GetValue() );
        }

        public void Execute() {
            Item1.SetValue( (int)State.X );
            Item2.SetValue( ( int )State.Y );
            Item3.SetValue( ( int )State.Z );
        }

        public void Redo() {
            Item1.SetValue( ( int )State.X );
            Item2.SetValue( ( int )State.Y );
            Item3.SetValue( ( int )State.Z );
        }

        public void Undo() {
            Item1.SetValue( ( int )PrevState.X );
            Item2.SetValue( ( int )PrevState.Y );
            Item3.SetValue( ( int )PrevState.Z );
        }
    }
}
