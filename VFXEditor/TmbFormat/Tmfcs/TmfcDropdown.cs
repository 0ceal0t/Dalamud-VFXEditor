using VfxEditor.FileManager;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Ui.Components;

namespace VfxEditor.TmbFormat.Tmfcs {
    public class TmfcDropdown : Dropdown<Tmfc> {
        public readonly TmbFile File;

        public TmfcDropdown( TmbFile file ) : base( "TMFC", file.Tmfcs, false, true ) {
            File = file;
        }

        protected override string GetText( Tmfc item, int idx ) => $"TMFC {idx}";

        protected override void OnDelete( Tmfc item ) {
            Plugin.AddModal( new TextModal(
                "Delete TMFC",
                "Are you sure you want to delete this item? This change is potentially detectable, so make sure you know what you're doing.",
                () => {
                    TmbRefreshIdsCommand command = new( File, false, true );
                    command.Add( new GenericRemoveCommand<Tmfc>( Items, item ) );
                    command.Add( new GenericRemoveCommand<TmbEntry>( File.AllEntries, item ) );
                    File.Command.Add( command );
                }
            ) );
        }

        protected override void OnNew() {
            var newTmfc = new Tmfc( File );
            TmbRefreshIdsCommand command = new( File, false, true );
            command.Add( new GenericAddCommand<Tmfc>( Items, newTmfc ) );
            command.Add( new GenericAddCommand<TmbEntry>( File.AllEntries, newTmfc ) );
            File.Command.Add( command );
        }

        protected override void DrawSelected() => Selected.DrawBody();
    }
}
