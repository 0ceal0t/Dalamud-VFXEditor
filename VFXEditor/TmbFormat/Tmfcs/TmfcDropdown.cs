using VfxEditor.FileManager;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Ui.Components;

namespace VfxEditor.TmbFormat.Tmfcs {
    public class TmfcDropdown : Dropdown<Tmfc> {
        public readonly TmbFile File;

        public TmfcDropdown( TmbFile file ) : base( "F-Curve", file.Tmfcs, false, true ) {
            File = file;
        }

        protected override string GetText( Tmfc item, int idx ) => $"F-Curve {idx}";

        protected override void OnDelete( Tmfc item ) {
            Plugin.AddModal( new TextModal(
                "Delete F-Curve",
                "Are you sure you want to delete this item? This change is potentially detectable, so make sure you know what you're doing.",
                () => {
                    var command = new TmbRefreshIdsCommand( File );
                    command.Add( new GenericRemoveCommand<Tmfc>( Items, item ) );
                    command.Add( new GenericRemoveCommand<TmbEntry>( File.AllEntries, item ) );
                    File.Command.Add( command );
                }
            ) );
        }

        protected override void OnNew() {
            var newTmfc = new Tmfc( File );
            var command = new TmbRefreshIdsCommand( File );
            command.Add( new GenericAddCommand<Tmfc>( Items, newTmfc ) );
            command.Add( new GenericAddCommand<TmbEntry>( File.AllEntries, newTmfc ) );
            File.Command.Add( command );
        }

        protected override void DrawSelected() => Selected.DrawBody();
    }
}
