using System.Numerics;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Ui.Components;

namespace VfxEditor.TmbFormat.Actor {
    public class TmbActorDropdown : Dropdown<Tmac> {
        private readonly TmbFile File;

        public TmbActorDropdown( TmbFile file ) : base( "Actor", file.Actors, true, true ) {
            File = file;
        }

        protected override string GetText( Tmac item, int idx ) => $"Actor {idx}";

        protected override bool DoColor( Tmac item, out Vector4 color ) => TmbEntry.DoColor( item.MaxDanger, out color );

        protected override void OnDelete( Tmac item ) {
            var command = new TmbRefreshIdsCommand( File );
            command.Add( new ListRemoveCommand<Tmac>( Items, item ) );
            command.Add( new ListRemoveCommand<Tmac>( File.HeaderTmal.Actors, item ) );
            item.DeleteChildren( command, File );
            CommandManager.Add( command );
        }

        protected override void OnNew() {
            var newActor = new Tmac( File );

            var command = new TmbRefreshIdsCommand( File );
            command.Add( new ListAddCommand<Tmac>( Items, newActor ) );
            command.Add( new ListAddCommand<Tmac>( File.HeaderTmal.Actors, newActor ) );
            CommandManager.Add( command );
        }

        protected override void DrawSelected() => Selected.Draw();
    }
}
