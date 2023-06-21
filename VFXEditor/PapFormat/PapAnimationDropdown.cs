using ImGuiFileDialog;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Interop;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public class PapAnimationDropdown : Dropdown<PapAnimation> {
        private readonly PapFile File;

        public PapAnimationDropdown( PapFile file, List<PapAnimation> items ) : base( "Animations", items, true, true ) {
            File = file;
        }

        protected override string GetText( PapAnimation item, int idx ) => item.GetName();

        protected override void OnDelete( PapAnimation item, CommandManager command ) {
            var index = Items.IndexOf( item );

            CompoundCommand command_ = new( false, true );
            command_.Add( new PapAnimationRemoveCommand( File, Items, item ) );
            command_.Add( new PapHavokFileCommand( File.HkxTempLocation, () => {
                HavokInterop.RemoveHavokAnimation( File.HkxTempLocation, index, File.HkxTempLocation );
            } ) );
            CommandManager.Pap.Add( command_ );

            UiUtils.OkNotification( "Havok data removed" );
        }

        protected override void OnNew( CommandManager command ) {
            FileDialogManager.OpenFileDialog( "Select a File", ".hkx,.*", ( bool ok, string res ) => {
                if( ok ) {
                    Plugin.PapManager.IndexDialog.OnOk = ( int idx ) => {
                        var newAnim = new PapAnimation( File.HkxTempLocation );
                        newAnim.ReadTmb( Path.Combine( Plugin.RootLocation, "Files", "default_pap_tmb.tmb" ), File.Command );

                        CompoundCommand command_ = new( false, true );
                        command_.Add( new PapAnimationAddCommand( File, Items, newAnim ) );
                        command_.Add( new PapHavokFileCommand( File.HkxTempLocation, () => {
                            HavokInterop.AddHavokAnimation( File.HkxTempLocation, res, idx, File.HkxTempLocation );
                        } ) );
                        CommandManager.Pap.Add( command_ );

                        UiUtils.OkNotification( "Havok data imported" );
                    };
                    Plugin.PapManager.IndexDialog.Show();
                }
            } );
        }

        protected override void DrawSelected() => Selected.Draw( File.ModelId.Value, File.ModelType.Value );
    }
}
