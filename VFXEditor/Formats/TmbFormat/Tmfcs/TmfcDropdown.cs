using System.Collections.Generic;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.Ui.Components;

namespace VfxEditor.TmbFormat.Tmfcs {
    public class TmfcDropdown : Dropdown<Tmfc> {
        public readonly TmbFile File;

        public TmfcDropdown( TmbFile file ) : base( "F-Curve", file.Tmfcs ) {
            File = file;
        }

        protected override string GetText( Tmfc item, int idx ) => $"F-Curve {idx}";

        protected override void DrawControls() => DrawNewDeleteControls( OnNew, OnDelete );

        private void OnNew() {
            var newTmfc = new Tmfc( File );
            var commands = new List<ICommand> {
                new ListAddCommand<Tmfc>( Items, newTmfc ),
                new ListAddCommand<TmbEntry>( File.AllEntries, newTmfc )
            };
            CommandManager.Add( new CompoundCommand( commands, File.RefreshIds ) );
        }

        private void OnDelete( Tmfc item ) {
            Plugin.AddModal( new TextModal(
                "Delete F-Curve",
                "Are you sure you want to delete this item? This change is potentially detectable, so make sure you know what you're doing.",
                () => {
                    var commands = new List<ICommand> {
                        new ListRemoveCommand<Tmfc>( Items, item ),
                        new ListRemoveCommand<TmbEntry>( File.AllEntries, item )
                    };
                    CommandManager.Add( new CompoundCommand( commands, File.RefreshIds ) );
                }
            ) );
        }

        protected override void DrawSelected() => Selected.DrawBody();
    }
}
