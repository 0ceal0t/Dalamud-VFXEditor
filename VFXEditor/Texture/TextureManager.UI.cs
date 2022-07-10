using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Numerics;
using VFXEditor.Helper;

namespace VFXEditor.Texture {
    public partial class TextureManager {
        private string NewCustomPath = string.Empty;
        private int PngMip = 9;
        private TextureFormat PngFormat = TextureFormat.DXT5;
        private static readonly TextureFormat[] ValidPngFormat = new[] { TextureFormat.DXT5, TextureFormat.DXT3, TextureFormat.DXT1, TextureFormat.A8, TextureFormat.A8R8G8B8 };

        public override void DrawBody() {
            var id = "##ImportTex";

            ImGui.SetNextItemWidth( ImGui.GetWindowContentRegionWidth() - 140 );
            ImGui.InputText( $"Path{id}-Input", ref NewCustomPath, 255 );

            ImGui.SameLine();
            if( ImGui.Button( $"Import Texture{id}" ) ) {
                var path = NewCustomPath.Trim().Trim( '\0' ).ToLower();
                if( !string.IsNullOrEmpty( path ) && !PathToTextureReplace.ContainsKey( path ) ) {
                    ImportDialog( path );
                }
            }

            ImGui.SetNextItemWidth( 200 );
            ImGui.InputInt( $"PNG Mip Levels{id}", ref PngMip );
            ImGui.SameLine();
            ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X - 200 );
            if( UIHelper.EnumComboBox( $"PNG Format{id}", ValidPngFormat, PngFormat, out var newPngFormat ) ) {
                PngFormat = newPngFormat;
            }

            // ======= DISPLAY IMPORTED TEXTURES =============

            ImGui.BeginChild( id + "/Child", new Vector2( -1, -1 ), true );

            if( PathToTextureReplace.IsEmpty ) {
                ImGui.Text( "No textures have been imported..." );
            }

            var idx = 0;
            foreach( var entry in PathToTextureReplace ) {
                if( ImGui.CollapsingHeader( $"{entry.Key}##{id}-{idx}" ) ) {
                    ImGui.Indent();
                    DrawTexture( entry.Key + '\u0000', $"{id}-{idx}" );
                    ImGui.Unindent();
                }
                idx++;
            }

            ImGui.EndChild();
        }

        public void DrawTexture( string path, string id ) {
            if( GetPreviewTexture( path, out var texOut ) ) {
                ImGui.Image( texOut.Wrap.ImGuiHandle, new Vector2( texOut.Width, texOut.Height ) );
                ImGui.Text( $"Format: {texOut.Format}  MIPS: {texOut.MipLevels}  SIZE: {texOut.Width}x{texOut.Height}" );
                if( ImGui.Button( "Export" + id ) ) {
                    ImGui.OpenPopup( "Tex_Export" + id );
                }
                ImGui.SameLine();
                if( ImGui.Button( "Replace" + id ) ) {
                    ImportDialog( path.Trim( '\0' ) );
                }
                if( ImGui.BeginPopup( "Tex_Export" + id ) ) {
                    if( ImGui.Selectable( "PNG" + id ) ) {
                        SavePngDialog( path.Trim( '\0' ) );
                    }
                    if( ImGui.Selectable( "DDS" + id ) ) {
                        SaveDDSDialog( path.Trim( '\0' ) );
                    }
                    ImGui.EndPopup();
                }

                // Imported texture
                if( texOut.IsReplaced ) {
                    ImGui.SameLine();
                    if( UIHelper.RemoveButton( "Remove Replaced Texture" + id ) ) {
                        RemoveReplaceTexture( path.Trim( '\0' ) );
                        RefreshPreviewTexture( path.Trim( '\0' ) );
                    }
                }
            }
        }

        private void ImportDialog( string newPath ) {
            FileDialogManager.OpenFileDialog( "Select a File", "Image files{.png,.atex,.dds},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    if( !ImportTexture( res, newPath, pngMip: (ushort) PngMip, pngFormat: PngFormat ) ) {
                        PluginLog.Error( $"Could not import" );
                    }
                }
                catch( Exception e ) {
                    PluginLog.Error( "Could not import data", e );
                }
            } );
        }

        private void SavePngDialog( string texPath ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".png", "ExportedTexture", "png", ( bool ok, string res ) => {
                if( !ok ) return;
                var texFile = GetRawTexture( texPath );
                texFile.SaveAsPNG( res );
            } );
        }

        private void SaveDDSDialog( string texPath ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".dds", "ExportedTexture", "dds", ( bool ok, string res ) => {
                if( !ok ) return;
                var texFile = GetRawTexture( texPath );
                texFile.SaveAsDDS( res );
            } );
        }
    }
}
