using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System;
using TeximpNet;
using VfxEditor.FileBrowser;
using VfxEditor.Formats.TextureFormat.Ui;
using Surface = TeximpNet.Surface;

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
                    if( ImGui.Button( "Resize" ) ) {
                        ApplyEdit( surface => surface.Resize( ResizeInput[0], ResizeInput[1], ImageFilter.Box ) );
                    }
                }

                if( ImGui.Selectable( "Grayscale" ) ) {
                    ApplyEdit( surface => surface.ConvertTo( ImageConversion.ToGreyscale ) );
                }

                if( ImGui.Selectable( "Flip Horizontally" ) ) {
                    ApplyEdit( surface => surface.FlipHorizontally() );
                }

                if( ImGui.Selectable( "Flip Vertically" ) ) {
                    ApplyEdit( surface => surface.FlipVertically() );
                }

                if( ImGui.Selectable( "Rotate Left" ) ) {
                    ApplyEdit( surface => surface.Rotate( 90 ) );
                }

                if( ImGui.Selectable( "Rotate Right" ) ) {
                    ApplyEdit( surface => surface.Rotate( -90 ) );
                }

                ImGui.EndPopup();
            }

            DrawSettingsPopup();
        }

        private void ApplyEdit( Action<Surface> edit ) {
            try {
                var file = GetRawData();
                if( file == null ) return;
                var surface = file.GetPngData( out var pin );
                if( surface == null ) return;

                edit( surface );

                surface.SaveToFile( ImageFormat.PNG, TextureManager.TempPng );
                surface.Dispose();
                MemoryHelper.UnpinObject( pin );

                OnReplace( TextureManager.TempPng );
            }
            catch( Exception ex ) {
                Dalamud.Error( ex, "Could not edit image" );
            }
        }

        protected static void DrawSettingsCog() {
            ImGui.SameLine();
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( FontAwesomeIcon.Cog.ToIconString() ) ) ImGui.OpenPopup( "Settings" );
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
