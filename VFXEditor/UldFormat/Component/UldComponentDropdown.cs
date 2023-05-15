using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Component {
    public class UldComponentDropdown : Dropdown<UldComponent> {
        public UldComponentDropdown( List<UldComponent> items ) : base( "Component", items, true ) { }

        protected override void OnDelete( UldComponent item ) {
            if( Items.IndexOf( item ) == -1 ) return;
            CommandManager.Uld.Add( new GenericRemoveCommand<UldComponent>( Items, item ) );
        }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldComponent>( Items, new UldComponent( Items ) ) );
        }

        protected override string GetText( UldComponent item, int idx ) => item.GetText();

        protected override void DrawSelected() => Selected.Draw();
    }
}
