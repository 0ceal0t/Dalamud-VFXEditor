using Dalamud.Logging;
using VfxEditor.AVFXLib.Emitter;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEmitterDataExtraCommand : ICommand {
        private readonly UiEmitter Item;
        private readonly AVFXBase OldData;
        private readonly UiData OldUi;

        private AVFXBase NewData;
        private UiData NewUi;

        public UiEmitterDataExtraCommand( UiEmitter item ) {
            Item = item;
            OldUi = item.Data;
            OldData = item.Emitter.Data;
        }

        public void Execute() {
            Item.Emitter.SetType( Item.Emitter.EmitterVariety.GetValue() );
            Item.UpdateDataType(); // already disables the old one

            NewData = Item.Emitter.Data;
            NewUi = Item.Data;
        }

        public void Redo() {
            OldUi?.Disable();
            Item.Emitter.Data = NewData;
            Item.Data = NewUi;
            NewUi?.Enable();
        }

        public void Undo() {
            NewUi?.Disable();
            Item.Emitter.Data = OldData;
            Item.Data = OldUi;
            OldUi?.Enable();
        }
    }
}
