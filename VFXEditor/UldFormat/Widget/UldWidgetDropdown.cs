using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;
using VfxEditor.UldFormat.Component;

namespace VfxEditor.UldFormat.Widget {
    public class UldWidgetDropdown : Dropdown<UldWidget> {
        private readonly List<UldComponent> Components;

        public UldWidgetDropdown( List<UldWidget> items, List<UldComponent> components ) : base( items, true ) {
            Components = components;
        }

        protected override string GetText( UldWidget item, int idx ) => $"Widget {item.Id.Value}";

        protected override void OnDelete( UldWidget item ) {
            if( Items.IndexOf( item ) == -1 ) return;
            CommandManager.Uld.Add( new GenericRemoveCommand<UldWidget>( Items, item ) );
        }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldWidget>( Items, new UldWidget( Components ) ) );
        }

        public override void Draw( string id ) {
            base.Draw( id );
            if( Selected != null ) Selected.Draw( $"{id}{Items.IndexOf( Selected )}" );
            else ImGui.Text( "Select a component..." );
        }
    }
}
