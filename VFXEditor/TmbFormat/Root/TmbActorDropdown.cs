using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.TmbFormat {
    public class TmbActorDropdown : Dropdown<Tmac> {
        private readonly TmbFile File;

        public TmbActorDropdown( TmbFile file, List<Tmac> items ) : base( items, true ) {
            File = file;
        }

        protected override string GetText( Tmac item, int idx ) => $"Actor {idx}";

        protected override void OnDelete( Tmac item ) => File.Command.Add( new GenericRemoveCommand<Tmac>( Items, item ) );

        protected override void OnNew() => File.Command.Add( new GenericAddCommand<Tmac>( Items, new Tmac( File.PapEmbedded ) ) );

        public override void Draw( string id ) {
            base.Draw( id );
            if( Selected != null ) Selected.Draw( $"{id}{Items.IndexOf( Selected )}", File.Tracks, File.Entries );
            else ImGui.Text( "Select a timeline actor..." );
        }
    }
}
