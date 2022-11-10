using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiBinderDataExtraCommand : ICommand {
        private readonly UiBinder Item;
        private readonly AVFXBase OldData;
        private readonly UiData OldUi;

        private AVFXBase NewData;
        private UiData NewUi;

        public UiBinderDataExtraCommand( UiBinder item ) {
            Item = item;
            OldUi = item.Data;
            OldData = item.Binder.Data;
        }

        public void Execute() {
            Item.Binder.SetType( Item.Binder.BinderVariety.GetValue() );
            Item.UpdateDataType(); // already disables the old one

            NewData = Item.Binder.Data;
            NewUi = Item.Data;
        }

        public void Redo() {
            OldUi?.Disable();
            Item.Binder.Data = NewData;
            Item.Data = NewUi;
            NewUi?.Enable();
        }

        public void Undo() {
            NewUi?.Disable();
            Item.Binder.Data = OldData;
            Item.Data = OldUi;
            OldUi?.Enable();
        }
    }
}
