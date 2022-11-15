using System;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEnumCommand<T> : ICommand where T : Enum {
        private readonly AvfxEnum<T> Item;
        private readonly T State;
        private readonly T PrevState;
        private readonly ICommand ExtraCommand;

        public AvfxEnumCommand( AvfxEnum<T> item, T state, ICommand extraCommand ) {
            Item = item;
            State = state;
            PrevState = item.GetValue();
            ExtraCommand = extraCommand;
        }

        public void Execute() {
            Item.SetValue( State );
            ExtraCommand?.Execute();
        }

        public void Redo() {
            Item.SetValue( State );
            ExtraCommand?.Redo();
        }

        public void Undo() {
            Item.SetValue( PrevState );
            ExtraCommand?.Undo();
        }
    }
}
