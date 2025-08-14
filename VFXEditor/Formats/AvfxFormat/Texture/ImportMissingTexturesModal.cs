using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using VfxEditor.FileBrowser;
using VfxEditor.Ui.Components;

namespace VfxEditor.Formats.AvfxFormat.Texture {
    public class ImportMissingTexturesModal : Modal {
        private readonly List<string> Paths;

        public ImportMissingTexturesModal( List<string> paths ) : base( "Import Missing Textures", false ) {
            Paths = paths;
        }

        protected override void DrawBody() {
            using var child = ImRaii.Child( "Child", new( 600, 300 ) );
            foreach( var (path, idx) in Paths.WithIndex() ) {
                using var _ = ImRaii.PushId( idx );
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {
                        var extension = path.Split( '.' )[^1].Trim( '\0' );
                        FileBrowserManager.OpenFileModal( "Select a File", "Image files{.png,." + extension + ",.dds},.*", ( bool ok, string res ) => {
                            Show();
                            if( !ok ) return;
                            try {
                                Plugin.TextureManager.ReplaceTexture( res, path );
                                Paths.Remove( path );
                            }
                            catch( Exception e ) {
                                Dalamud.Error( e, "Could not import data" );
                            }
                        } );
                    }
                }

                ImGui.SameLine();
                ImGui.TextDisabled( path );
            }
        }

        protected override void OnCancel() { }
        protected override void OnOk() { }
    }
}
