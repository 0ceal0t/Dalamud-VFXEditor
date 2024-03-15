using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Ui.Components {
    public class UiDropdown<T> : Dropdown<T> where T : class, IUiItem {
        public UiDropdown( string id, List<T> items ) : base( id, items ) { }

        protected override void DrawSelected() {
            using var _ = ImRaii.PushId( Items.IndexOf( Selected ) );
            Selected.Draw();
        }
    }
}
