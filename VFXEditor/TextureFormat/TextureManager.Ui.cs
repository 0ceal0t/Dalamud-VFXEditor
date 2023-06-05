using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.TextureFormat {
    public partial class TextureManager {
        private string NewCustomPath = string.Empty;
        private int PngMip = 9;
        private TextureFormat PngFormat = TextureFormat.DXT5;

        private static readonly TextureFormat[] ValidPngFormat = new[] {
            TextureFormat.DXT5,
            TextureFormat.DXT3,
            TextureFormat.DXT1,
            TextureFormat.A8R8G8B8,
        };

        public override void DrawBody() {
            using var _ = ImRaii.PushId( "ImportTex" );

            // Input

            var imguiStyle = ImGui.GetStyle();
            var buttonSize = ImGui.CalcTextSize( "Import Texture" ).X + imguiStyle.FramePadding.X * 2 + imguiStyle.ItemInnerSpacing.X;
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 3, 4 ) ) ) {
                ImGui.SetNextItemWidth( UiUtils.GetWindowContentRegionWidth() - buttonSize );
                ImGui.InputTextWithHint( $"##GamePath", "Game Path", ref NewCustomPath, 255 );

                ImGui.SameLine();
                var path = NewCustomPath.Trim().Trim( '\0' );
                var importDisabled = string.IsNullOrEmpty( path ) || PathToTextureReplace.ContainsKey( path );
                using var dimmed = ImRaii.PushStyle( ImGuiStyleVar.Alpha, 0.5f, importDisabled );
                if( ImGui.Button( "Import Texture" ) && !importDisabled ) {
                    ImportDialog( path );
                }
            }

            // Mip

            ImGui.SetNextItemWidth( 150 );
            ImGui.InputInt( "PNG Mip Levels", ref PngMip );

            // Format

            ImGui.SameLine();
            ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X - buttonSize );
            if( UiUtils.EnumComboBox( "PNG Format", ValidPngFormat, PngFormat, out var newPngFormat ) ) {
                PngFormat = newPngFormat;
            }

            // ======= DISPLAY IMPORTED TEXTURES =============

            using var child = ImRaii.Child( "Child", new Vector2( -1, -1 ), true );

            if( PathToTextureReplace.IsEmpty ) ImGui.TextDisabled( "No textures have been imported..." );

            var idx = 0;
            foreach( var entry in PathToTextureReplace ) {
                using var __ = ImRaii.PushId( idx );
                if( ImGui.CollapsingHeader( entry.Key ) ) {
                    using var indent = ImRaii.PushIndent();
                    DrawTexture( entry.Key );
                }
                idx++;
            }
        }

        public void DrawTexture( string path ) {
            if( GetPreviewTexture( path, out var texture ) ) {
                ImGui.Image( texture.Wrap.ImGuiHandle, new Vector2( texture.Width, texture.Height ) );
                ImGui.TextDisabled( $"Format: {texture.Format}  Mips: {texture.MipLevels}  Size: {texture.Width}x{texture.Height}" );

                if( ImGui.Button( "Export" ) ) ImGui.OpenPopup( "TexExport" );
                ImGui.SameLine();
                if( ImGui.Button( "Replace" ) ) ImportDialog( path );
                if( ImGui.BeginPopup( "TexExport" ) ) {
                    if( ImGui.Selectable( "PNG" ) ) SavePngDialog( path );
                    if( ImGui.Selectable( "DDS" ) ) SaveDdsDialog( path );
                    ImGui.EndPopup();
                }

                if( texture.IsReplaced ) {
                    ImGui.SameLine();
                    if( UiUtils.RemoveButton( "Remove Replaced Texture" ) ) {
                        RemoveReplaceTexture( path );
                        RefreshPreviewTexture( path );
                    }
                }
            }
        }

        public void DrawTextureUv( string path, uint u, uint v, uint w, uint h ) {
            if( GetPreviewTexture( path, out var texture ) ) {
                var size = new Vector2( texture.Width, texture.Height );
                var uv0 = new Vector2( u, v ) / size;
                var uv1 = uv0 + new Vector2( w, h ) / size;
                ImGui.Image( texture.Wrap.ImGuiHandle, new Vector2( w, h ), uv0, uv1 );
            }
        }

        private void ImportDialog( string newPath ) {
            FileDialogManager.OpenFileDialog( "Select a File", "Image files{.png,." + newPath.Split( '.' )[^1].Trim( '\0' ) + ",.dds},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    if( !ImportTexture( res, newPath, pngMip: ( ushort )PngMip, pngFormat: PngFormat ) ) PluginLog.Error( $"Could not import" );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not import data" );
                }
            } );
        }

        private void SavePngDialog( string texPath ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".png", "ExportedTexture", "png", ( bool ok, string res ) => {
                if( !ok ) return;
                var texFile = GetRawTexture( texPath );
                texFile.SaveAsPng( res );
            } );
        }

        private void SaveDdsDialog( string texPath ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".dds", "ExportedTexture", "dds", ( bool ok, string res ) => {
                if( !ok ) return;
                var texFile = GetRawTexture( texPath );
                texFile.SaveAsDds( res );
            } );
        }
    }
}
