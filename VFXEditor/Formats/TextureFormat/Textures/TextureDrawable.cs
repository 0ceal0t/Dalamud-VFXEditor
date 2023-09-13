using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;

namespace VfxEditor.Formats.TextureFormat.Textures {
    public abstract class TextureDrawable {
        protected string GamePath = "";
        protected string GameExtension => GamePath.Split( '.' )[^1].Trim( '\0' );

        public TextureDrawable( string gamePath ) {
            GamePath = gamePath.Trim( '\0' );
        }

        protected abstract TextureDataFile GetRawData();

        protected abstract void OnReplace( string importPath );

        public abstract void DrawImage();
        public abstract void DrawImage( uint u, uint v, uint w, uint h );

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
            if( ImGui.Button( "Export" ) ) ImGui.OpenPopup( "TexExport" );

            ImGui.SameLine();
            if( ImGui.Button( "Replace" ) ) ImportDialog();

            if( ImGui.BeginPopup( "TexExport" ) ) {
                if( ImGui.Selectable( "PNG" ) ) SavePngDialog();
                if( ImGui.Selectable( "DDS" ) ) SaveDdsDialog();
                ImGui.EndPopup();
            }
        }

        protected void ImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", "Image files{.png,." + GameExtension + ",.dds},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    OnReplace( res );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not import data" );
                }
            } );
        }

        protected void SavePngDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".png", "ExportedTexture", "png", ( bool ok, string res ) => {
                if( !ok ) return;
                GetRawData()?.SaveAsPng( res );
            } );
        }

        protected void SaveDdsDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".dds", "ExportedTexture", "dds", ( bool ok, string res ) => {
                if( !ok ) return;
                GetRawData()?.SaveAsDds( res );
            } );
        }
    }
}
