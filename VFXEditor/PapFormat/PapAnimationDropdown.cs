using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Interop;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public class PapAnimationDropdown : Dropdown<PapAnimation> {
        private readonly PapFile File;

        public PapAnimationDropdown( PapFile file, List<PapAnimation> items ) : base( items, true ) {
            File = file;
        }

        protected override string GetText( PapAnimation item, int idx ) => item.GetName();

        protected override void OnDelete( PapAnimation item ) {
            var index = Items.IndexOf( item );
            if( index == -1 ) return;

            CompoundCommand command = new( false, true );
            command.Add( new PapAnimationRemoveCommand( File, Items, item ) );
            command.Add( new PapHavokFileCommand( File.HkxTempLocation, () => {
                HavokInterop.RemoveHavokAnimation( File.HkxTempLocation, index, File.HkxTempLocation );
            } ) );
            CommandManager.Pap.Add( command );

            UiUtils.OkNotification( "Havok data removed" );
        }

        protected override void OnNew() {
            FileDialogManager.OpenFileDialog( "Select a File", ".hkx,.*", ( bool ok, string res ) => {
                if( ok ) {
                    Plugin.PapManager.IndexDialog.OnOk = ( int idx ) => {
                        var newAnim = new PapAnimation( File.HkxTempLocation );
                        newAnim.ReadTmb( Path.Combine( Plugin.RootLocation, "Files", "default_pap_tmb.tmb" ) );

                        CompoundCommand command = new( false, true );
                        command.Add( new PapAnimationAddCommand( File, Items, newAnim ) );
                        command.Add( new PapHavokFileCommand( File.HkxTempLocation, () => {
                            HavokInterop.AddHavokAnimation( File.HkxTempLocation, res, idx, File.HkxTempLocation );
                        } ) );
                        CommandManager.Pap.Add( command );

                        UiUtils.OkNotification( "Havok data imported" );
                    };
                    Plugin.PapManager.IndexDialog.Show();
                }
            } );
        }

        public override void Draw( string id ) {
            base.Draw( id );
            if( Selected != null ) {
                Selected.Draw( $"{id}{Items.IndexOf( Selected )}", File.ModelId.Value, File.ModelType.GetValue() );
            }
            else ImGui.Text( "Select an animation..." );
        }
    }
}
