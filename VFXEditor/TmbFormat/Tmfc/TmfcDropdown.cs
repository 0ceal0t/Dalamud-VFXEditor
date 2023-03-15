using ImGuiNET;
using System;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat {
    public class TmfcDropdown : Dropdown<Tmfc> {
        private readonly TmbFile File;

        public TmfcDropdown( TmbFile file ) : base( file.Tmfcs, false ) {
            File = file;
        }

        protected override string GetText( Tmfc item, int idx ) => $"TMFC {idx}";

        protected override void OnDelete( Tmfc item ) {
            TmbRefreshIdsCommand command = new( File, false, true );
            command.Add( new GenericRemoveCommand<Tmfc>( Items, item ) );
            command.Add( new GenericRemoveCommand<TmbEntry>( File.Entries, item ) );
            File.Command.Add( command );
        }

        protected override void OnNew() {
            var newTmfc = new Tmfc( File.PapEmbedded );
            TmbRefreshIdsCommand command = new( File, false, true );
            command.Add( new GenericAddCommand<Tmfc>( Items, newTmfc ) );
            command.Add( new GenericAddCommand<TmbEntry>( File.Entries, newTmfc ) );
            File.Command.Add( command );
        }

        public override void Draw( string id ) {
            base.Draw( id );
            ImGui.TextDisabled( "[WORK IN PROGRESS]" );
            if( Selected != null ) Selected.Draw( $"{id}{Items.IndexOf( Selected )}" );
            else ImGui.Text( "Select a TMFC..." );
        }
    }
}
