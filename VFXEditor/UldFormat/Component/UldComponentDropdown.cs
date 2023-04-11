using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Component {
    public class UldComponentDropdown : Dropdown<UldComponent> {
        public UldComponentDropdown( List<UldComponent> items ) : base( items, true ) { }

        protected override string GetText( UldComponent item, int idx ) => $"Component {item.Id.Value} ({item.Type.Value})";

        protected override void OnDelete( UldComponent item ) {
            if( Items.IndexOf( item ) == -1 ) return;
            CommandManager.Uld.Add( new GenericRemoveCommand<UldComponent>( Items, item ) );
        }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldComponent>( Items, new UldComponent( Items ) ) );
        }

        public override void Draw( string id ) {
            base.Draw( id );
            if( Selected != null ) Selected.Draw( $"{id}{Items.IndexOf( Selected )}" );
            else ImGui.Text( "Select a component..." );
        }
    }
}
