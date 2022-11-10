using System;
using System.Numerics;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiFloat2Command : ICommand {
        private readonly AVFXFloat Item1;
        private readonly AVFXFloat Item2;

        private readonly Vector2 State;
        private readonly Vector2 PrevState;

        public UiFloat2Command( AVFXFloat item1, AVFXFloat item2, Vector2 state ) {
            Item1 = item1;
            Item2 = item2;
            State = state;
            PrevState = new Vector2( item1.GetValue(), item2.GetValue() );
        }

        public void Execute() {
            Item1.SetValue( State.X );
            Item2.SetValue( State.Y );
        }

        public void Redo() {
            Item1.SetValue( State.X );
            Item2.SetValue( State.Y );
        }

        public void Undo() {
            Item1.SetValue( PrevState.X );
            Item2.SetValue( PrevState.Y );
        }
    }
}
