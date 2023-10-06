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
            if( ImGui.Button( "Export" ) ) ImGui.OpenPopup( "TexExport" );

            ImGui.SameLine();
            if( ImGui.Button( "Replace" ) ) ImportDialog();

            if( ImGui.BeginPopup( "TexExport" ) ) {
                if( ImGui.Selectable( ".png" ) ) GetRawData()?.SavePngDialog();
                if( ImGui.Selectable( ".dds" ) ) GetRawData()?.SaveDdsDialog();
                if( ImGui.Selectable( $".{GameExtension}" ) ) GetRawData()?.SaveTexDialog( GameExtension );
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
                    Dalamud.Error( e, "Could not import data" );
                }
            } );
        }
    }
}
