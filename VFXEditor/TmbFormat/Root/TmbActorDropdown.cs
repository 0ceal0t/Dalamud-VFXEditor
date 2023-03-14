using ImGuiNET;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.TmbFormat {
    public class TmbActorDropdown : Dropdown<Tmac> {
        private readonly TmbFile File;

        public TmbActorDropdown( TmbFile file ) : base( file.Actors, true ) {
            File = file;
        }

        protected override string GetText( Tmac item, int idx ) => $"Actor {idx}";

        protected override void OnDelete( Tmac item ) {
            CompoundCommand command = new( false, false );
            command.Add( new GenericRemoveCommand<Tmac>( Items, item ) );
            command.Add( File.GetRefreshIdsCommand() );
            File.Command.Add( command );
        }

        protected override void OnNew() {
            CompoundCommand command = new( false, false );
            command.Add( new GenericAddCommand<Tmac>( Items, new Tmac( File.PapEmbedded ) ) );
            command.Add( File.GetRefreshIdsCommand() );
            File.Command.Add( command );
        }

        public override void Draw( string id ) {
            base.Draw( id );
            if( Selected != null ) Selected.Draw( $"{id}{Items.IndexOf( Selected )}", File );
            else ImGui.Text( "Select a timeline actor..." );
        }
    }
}
