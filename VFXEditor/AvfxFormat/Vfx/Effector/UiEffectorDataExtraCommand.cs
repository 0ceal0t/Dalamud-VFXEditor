using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEffectorDataExtraCommand : ICommand {
        private readonly UiEffector Item;
        private readonly AVFXBase OldData;
        private readonly UiData OldUi;

        private AVFXBase NewData;
        private UiData NewUi;

        public UiEffectorDataExtraCommand( UiEffector item ) {
            Item = item;
            OldUi = item.Data;
            OldData = item.Effector.Data;
        }

        public void Execute() {
            Item.Effector.SetType( Item.Effector.EffectorVariety.GetValue() );
            Item.UpdateDataType(); // already disables the old one

            NewData = Item.Effector.Data;
            NewUi = Item.Data;
        }

        public void Redo() {
            OldUi?.Disable();
            Item.Effector.Data = NewData;
            Item.Data = NewUi;
            NewUi?.Enable();
        }

        public void Undo() {
            NewUi?.Disable();
            Item.Effector.Data = OldData;
            Item.Data = OldUi;
            OldUi?.Enable();
        }
    }
}
