using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;
using VfxEditor.TmbFormat.Entries;

namespace VfxEditor.TmbFormat {
    public class TmfcDropdown : Dropdown<Tmfc> {
        private readonly TmbFile File;
        private readonly List<TmbEntry> Entries;

        public TmfcDropdown( TmbFile file, List<Tmfc> items, List<TmbEntry> entries ) : base( items, true ) {
            File = file;
            Entries = entries;
        }

        protected override string GetText( Tmfc item, int idx ) => $"TMFC {idx}";

        protected override void OnDelete( Tmfc item ) {
            var command = new CompoundCommand( false, true );
            command.Add( new GenericRemoveCommand<Tmfc>( Items, item ) );
            command.Add( new GenericRemoveCommand<TmbEntry>( Entries, item ) );
            File.Command.Add( command );
        }

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
