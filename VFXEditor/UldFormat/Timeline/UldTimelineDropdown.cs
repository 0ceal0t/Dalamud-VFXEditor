using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Timeline {
    public class UldTimelineDropdown : Dropdown<UldTimeline> {
        public UldTimelineDropdown( List<UldTimeline> items ) : base( items, true ) { }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldTimeline>( Items, new UldTimeline() ) );
        }

        protected override void OnDelete( UldTimeline item ) {
            CommandManager.Uld.Add( new GenericRemoveCommand<UldTimeline>( Items, item ) );
        }

        public override void Draw( string id ) {
            base.Draw( id );
            if( Selected != null ) Selected.Draw( $"{id}{Items.IndexOf( Selected )}" );
            else ImGui.Text( "Select a timeline..." );
        }

        protected override string GetText( UldTimeline item, int idx ) => $"Timeline {item.Id.Value}";
    }
}
