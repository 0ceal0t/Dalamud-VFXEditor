using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ImGuiFileDialog;
using ImGuiNET;

using VFXEditor.Data;
using VFXEditor.Helper;
using VFXEditor.Penumbra;
using VFXEditor.Textools;
using VFXEditor.Texture;
using VFXEditor.Tmb;
using VFXEditor.Tracker;
using VFXEditor.Avfx;

namespace VFXEditor {
    public partial class Plugin {
        public void Draw() {
            if( !AvfxManager.IsVisible ) return;
            if( IsLoading ) return;

            CopyManager.PreDraw();

            TexToolsDialog.Draw();
            PenumbraDialog.Draw();
            ToolsDialog.Draw();
            VfxTracker.Draw();
            Configuration.Draw();
            AvfxManager.Draw();
            TextureManager.Draw();
            TmbManager.Draw();
        }

        public static void DrawMenu() {
            if( ImGui.BeginMenu( "File##Menu" ) ) {
                ImGui.TextDisabled( "Workspace" );
                ImGui.SameLine();
                UiHelper.HelpMarker( "A workspace allows you to save multiple vfx replacements at the same time, as well as any imported textures or item renaming (such as particles or emitters)" );

                if( ImGui.MenuItem( "New##Menu" ) ) NewWorkspace();
                if( ImGui.MenuItem( "Open##Menu" ) ) OpenWorkspace();
                if( ImGui.MenuItem( "Save##Menu" ) ) SaveWorkspace();
                if( ImGui.MenuItem( "Save As##Menu" ) ) SaveAsWorkspace();

                ImGui.Separator();
                if( ImGui.MenuItem( "Settings##Menu" ) ) Configuration.Show();
                if( ImGui.MenuItem( "Tools##Menu" ) ) ToolsDialog.Draw();
                if( ImGui.BeginMenu( "Help##Menu" ) ) {
                    if( ImGui.MenuItem( "Report an Issue##Menu" ) ) OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues" );
                    if( ImGui.MenuItem( "Guides##Menu" ) ) OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );
                    if( ImGui.MenuItem( "Github##Menu" ) ) OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor" );
                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }
            if( ImGui.BeginMenu( "Edit##Menu" ) ) {
                if( ImGui.MenuItem( "Copy##Menu" ) ) CopyManager.Copy();
                if( ImGui.MenuItem( "Paste##Menu" ) ) CopyManager.Paste();
                ImGui.EndMenu();
            }
            if( ImGui.BeginMenu( "Mod Export##Menu" ) ) {
                if( ImGui.MenuItem( "Penumbra##Menu" ) ) PenumbraDialog.Show();
                if( ImGui.MenuItem( "Textools##Menu" ) ) TexToolsDialog.Show();
                ImGui.EndMenu();
            }
            if( ImGui.MenuItem( "Vfx##Menu" ) ) AvfxManager.Show();
            if( ImGui.MenuItem( "Textures##Menu" ) ) TextureManager.Show();
            if( ImGui.MenuItem( "Tmb##Menu " ) ) TmbManager.Show();
        }

        public static void WriteBytesDialog( string filter, string data, string ext ) {
            WriteBytesDialog( filter, Encoding.ASCII.GetBytes( data ), ext );
        }

        public static void WriteBytesDialog( string filter, byte[] data, string ext ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", filter, "", ext, ( bool ok, string res ) => {
                if( ok ) File.WriteAllBytes( res, data );
            } );
        }

        public static void OpenUrl( string url ) {
            Process.Start( new ProcessStartInfo {
                FileName = url,
                UseShellExecute = true
            } );
        }
    }
}