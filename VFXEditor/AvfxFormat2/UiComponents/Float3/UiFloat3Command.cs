using System;
using System.Numerics;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat2 {
    public class UiFloat3Command : ICommand {
        private readonly AvfxFloat Item1;
        private readonly AvfxFloat Item2;
        private readonly AvfxFloat Item3;

        private readonly Vector3 State;
        private readonly Vector3 PrevState;

        public UiFloat3Command( AvfxFloat item1, AvfxFloat item2, AvfxFloat item3, Vector3 state ) {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            State = state;
            PrevState = new Vector3( item1.GetValue(), item2.GetValue(), item3.GetValue() );
        }

        public void Execute() {
            Item1.SetValue( State.X );
            Item2.SetValue( State.Y );
            Item3.SetValue( State.Z );
        }

        public void Redo() {
            Item1.SetValue( State.X );
            Item2.SetValue( State.Y );
            Item3.SetValue( State.Z );
        }

        public void Undo() {
            Item1.SetValue( PrevState.X );
            Item2.SetValue( PrevState.Y );
            Item3.SetValue( PrevState.Z );
        }
    }
}
