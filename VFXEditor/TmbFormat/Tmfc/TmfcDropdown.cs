using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;
using VfxEditor.TmbFormat.Entries;

namespace VfxEditor.TmbFormat {
    public class TmfcDropdown : Dropdown<Tmfc> {
        private readonly TmbFile File;

        public TmfcDropdown( TmbFile file, List<Tmfc> items ) : base( items, true ) {
            File = file;
        }

        protected override string GetText( Tmfc item, int idx ) => $"TMFC {idx}";

        // TODO: remove from entries as well
        protected override void OnDelete( Tmfc item ) => File.Command.Add( new GenericRemoveCommand<Tmfc>( Items, item ) );

        // TODO: add to entries as well
        protected override void OnNew() => File.Command.Add( new GenericAddCommand<Tmfc>( Items, new Tmfc( File.PapEmbedded ) ) );

        public override void Draw( string id ) {
            base.Draw( id );
            ImGui.TextDisabled( "[WORK IN PROGRESS]" );
            if( Selected != null ) Selected.Draw( $"{id}{Items.IndexOf( Selected )}" );
            else ImGui.Text( "Select a TMFC..." );
        }
    }
}
