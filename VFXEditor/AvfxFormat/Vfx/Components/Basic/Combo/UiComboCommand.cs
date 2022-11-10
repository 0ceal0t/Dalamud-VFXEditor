using System;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiComboCommand<T> : ICommand {
        private readonly AVFXEnum<T> Item;
        private readonly T State;
        private readonly T PrevState;
        private readonly ICommand ExtraCommand;

        public UiComboCommand( AVFXEnum<T> item, T state, ICommand extraCommand ) {
            Item = item;
            State = state;
            PrevState = item.GetValue();
            ExtraCommand = extraCommand; // Save current data
        }

        public void Execute() {
            Item.SetValue( State );
            ExtraCommand?.Execute(); // Disable current data, replace it
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
