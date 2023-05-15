using VfxEditor.FileManager;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Ui.Components;

namespace VfxEditor.TmbFormat {
    public class TmfcDropdown : Dropdown<Tmfc> {
        private readonly TmbFile File;

        public TmfcDropdown( TmbFile file ) : base( "TMFC", file.Tmfcs, false ) {
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

        protected override void DrawSelected() => Selected.Draw();
    }
}
