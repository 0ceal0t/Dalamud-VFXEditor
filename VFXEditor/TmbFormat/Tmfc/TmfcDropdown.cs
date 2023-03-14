using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;
using VfxEditor.TmbFormat.Entries;
using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.TmbFormat {
    public class TmfcDropdown : Dropdown<Tmfc> {
        private readonly TmbFile File;

        public TmfcDropdown( TmbFile file ) : base( file.Tmfcs, true ) {
            File = file;
        }

        protected override string GetText( Tmfc item, int idx ) => $"TMFC {idx}";

        protected override void OnDelete( Tmfc item ) {
            CompoundCommand command = new( false, false );
            command.Add( new GenericRemoveCommand<Tmfc>( Items, item ) );
            command.Add( new GenericRemoveCommand<TmbEntry>( File.Entries, item ) );
            command.Add( File.GetRefreshIdsCommand() );
            File.Command.Add( command );
        }

        protected override void OnNew() {
            var newTmfc = new Tmfc( File.PapEmbedded );
            CompoundCommand command = new( false, false );
            command.Add( new GenericAddCommand<Tmfc>( Items, newTmfc ) );
            command.Add( new GenericAddCommand<TmbEntry>( File.Entries, newTmfc ) );
            command.Add( File.GetRefreshIdsCommand() );
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
