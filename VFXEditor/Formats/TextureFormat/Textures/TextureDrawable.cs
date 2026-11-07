using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using OtterTex;
using System;
using VfxEditor.FileBrowser;
using VfxEditor.Formats.TextureFormat.Ui;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Textures {
    public abstract class TextureDrawable {
        protected string GamePath = "";

        private int[] ResizeInput;

        public TextureDrawable( string gamePath ) {
            GamePath = gamePath.Trim( '\0' );
        }

        protected abstract TexturePreview GetPreview();
        protected abstract TextureDataFile GetRawData();

        protected abstract void OnReplace( string importPath );

        public abstract void DrawFullImage();
        public abstract void DrawImage();
        public abstract void DrawImage( uint u, uint v, uint w, uint h );
        public abstract void DrawImage( float height );

        protected abstract void DrawControls();

        public void Draw() {
            DrawImage();
            DrawControls();
        }

        public void Draw( uint u, uint v, uint w, uint h, bool controls ) {
            DrawImage( u, v, w, h );
            if( controls ) DrawControls();
        }

        protected void DrawExportReplaceButtons() {
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                if( ImGui.Button( "Export" ) ) ImGui.OpenPopup( "TexExport" );

                ImGui.SameLine();
                if( ImGui.Button( "Replace" ) ) ImportDialog();

                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( ImGui.Button( FontAwesomeIcon.Edit.ToIconString() ) ) ImGui.OpenPopup( "Edit" );
                    UiUtils.Tooltip( "Edit texture" );
                }

                DrawSettingsCog();
            }

            if( ImGui.BeginPopup( "TexExport" ) ) {
                if( ImGui.Selectable( ".png" ) ) GetRawData()?.SavePngDialog();
                if( ImGui.Selectable( ".dds" ) ) GetRawData()?.SaveDdsDialog();
                if( ImGui.Selectable( ".atex" ) ) GetRawData()?.SaveTexDialog( "atex" );
                if( ImGui.Selectable( ".tex" ) ) GetRawData()?.SaveTexDialog( "tex" );
                ImGui.EndPopup();
            }

            if( ImGui.BeginPopup( "Edit" ) ) {
                if( ResizeInput == null && GetPreview() != null ) ResizeInput = [GetPreview().Width, GetPreview().Height];
                ImGui.SetNextItemWidth( 100f );
                ImGui.InputInt( "##Resize", ResizeInput );
                using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                    ImGui.SameLine();
                    if( ImGui.Button( "Resize" ) && ResizeInput[0] > 0 && ResizeInput[1] > 0 ) {
                        ApplyEdit( image => image.Resize( ResizeInput[0], ResizeInput[1], FilterFlags.Cubic | FilterFlags.ForceNonWIC ) );
                    }
                }

                if( ImGui.Selectable( "Grayscale" ) ) {
                    ApplyEdit( Grayscale );
                }

                if( ImGui.Selectable( "Flip Horizontally" ) ) {
                    ApplyEdit( image => image.FlipRotate( FlipRotateFlags.FlipHorizontal ) );
                }

                if( ImGui.Selectable( "Flip Vertically" ) ) {
                    ApplyEdit( image => image.FlipRotate( FlipRotateFlags.FlipVertical ) );
                }

                // DirectXTex rotates clockwise
                if( ImGui.Selectable( "Rotate Left" ) ) {
                    ApplyEdit( image => image.FlipRotate( FlipRotateFlags.Rotate270 ) );
                }

                if( ImGui.Selectable( "Rotate Right" ) ) {
                    ApplyEdit( image => image.FlipRotate( FlipRotateFlags.Rotate90 ) );
                }

                ImGui.EndPopup();
            }

            DrawSettingsPopup();
        }

        private void ApplyEdit( Func<ScratchImage, ScratchImage> edit ) {
            try {
                var file = GetRawData();
                if( file == null ) return;

                // Keeps the texture's original format and mip count
                file.SaveEdited( TextureManager.TempEdit, edit );
                OnReplace( TextureManager.TempEdit );
            }
            catch( Exception ex ) {
                Dalamud.Error( ex, "Could not edit image" );
            }
        }

        private static ScratchImage Grayscale( ScratchImage image ) {
            var rgba = image.Pixels.ToArray();
            for( var i = 0; i < rgba.Length; i += 4 ) {
                var luma = ( byte )Math.Round( 0.2126 * rgba[i] + 0.7152 * rgba[i + 1] + 0.0722 * rgba[i + 2] );
                rgba[i] = rgba[i + 1] = rgba[i + 2] = luma;
            }
            return ScratchImage.FromRGBA( rgba, image.Meta.Width, image.Meta.Height );
        }

        protected static void DrawSettingsCog() {
            ImGui.SameLine();
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( FontAwesomeIcon.Cog.ToIconString() ) ) ImGui.OpenPopup( "Settings" );
            UiUtils.Tooltip( "Texture settings" );
        }

        protected static void DrawSettingsPopup() {
            using var popup = ImRaii.Popup( "Settings" );
            if( !popup ) return;

            ImGui.TextDisabled( ".png Import Settings" );

            TextureView.DrawPngSettings();
        }

        protected void ImportDialog() {
            FileBrowserManager.OpenFileDialog( "Select a File", "Image files{.png,.atex,.tex,.dds},.*", ( ok, res ) => {
                if( !ok ) return;
                try {
                    OnReplace( res );
                }
                catch( Exception e ) {
                    Dalamud.Error( e, "Could not import data" );
                }
            } );
        }
    }
}