using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.EidFormat {
    public class EidBindPointDropdown : Dropdown<EidBindPoint> {
        public EidBindPointDropdown( List<EidBindPoint> items ) : base( items, true ) { }

        protected override string GetText( EidBindPoint item, int idx ) => $"Bind Point {item.BindPointId}";

        protected override void OnDelete( EidBindPoint item ) {
            if( Items.IndexOf( item ) == -1 ) return;
            CommandManager.Eid.Add( new GenericRemoveCommand<EidBindPoint>( Items, item ) );
        }

        protected override void OnNew() {
            CommandManager.Eid.Add( new GenericAddCommand<EidBindPoint>( Items, new EidBindPoint() ) );
        }

        public override void Draw( string id ) {
            base.Draw( id );
            if( Selected != null ) Selected.Draw( $"{id}{Items.IndexOf( Selected )}" );
            else ImGui.Text( "Select a bind point..." );
        }
    }
}
