using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using TeximpNet;
using VfxEditor.FileBrowser;
using VfxEditor.Formats.TextureFormat.Ui;
using Surface = TeximpNet.Surface;

namespace VfxEditor.Formats.TextureFormat.Textures {
    public abstract class TextureDrawable {
        protected string GamePath = "";
        protected string GameExtension => GamePath.Split( '.' )[^1].Trim( '\0' );

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

        public void Draw( uint u, uint v, uint w, uint h ) {
            DrawImage( u, v, w, h );
            DrawControls();
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
                if( ImGui.Selectable( $".{GameExtension}" ) ) GetRawData()?.SaveTexDialog( GameExtension );
                ImGui.EndPopup();
            }

            if( ImGui.BeginPopup( "Edit" ) ) {
                if( ResizeInput == null && GetPreview() != null ) ResizeInput = new int[] { GetPreview().Width, GetPreview().Height };
                ImGui.SetNextItemWidth( 100f );
                ImGui.InputInt2( "##Resize", ref ResizeInput[0] );
                using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                    ImGui.SameLine();
                    if( ImGui.Button( "Resize" ) ) {
                        ApplyEdit( ( Surface surface ) => surface.Resize( ResizeInput[0], ResizeInput[1], ImageFilter.Box ) );
                    }
                }

                if( ImGui.Selectable( "Grayscale" ) ) {
                    ApplyEdit( ( Surface surface ) => surface.ConvertTo( ImageConversion.ToGreyscale ) );
                }

                if( ImGui.Selectable( "Flip Horizontally" ) ) {
                    ApplyEdit( ( Surface surface ) => surface.FlipHorizontally() );
                }

                if( ImGui.Selectable( "Flip Vertically" ) ) {
                    ApplyEdit( ( Surface surface ) => surface.FlipVertically() );
                }

                if( ImGui.Selectable( "Rotate Left" ) ) {
                    ApplyEdit( ( Surface surface ) => surface.Rotate( 90 ) );
                }

                if( ImGui.Selectable( "Rotate Right" ) ) {
                    ApplyEdit( ( Surface surface ) => surface.Rotate( -90 ) );
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
            FileBrowserManager.OpenFileDialog( "Select a File", "Image files{.png,." + GameExtension + ",.dds},.*", ( bool ok, string res ) => {
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
