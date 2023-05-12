using Dalamud.Logging;
using ImGuiNET;
using System;
using VfxEditor.Data;
using VfxEditor.Utils;
using VfxEditor.Penumbra;
using VfxEditor.TexTools;
using VfxEditor.TextureFormat;
using VfxEditor.Tracker;
using VfxEditor.FileManager;
using OtterGui.Raii;

namespace VfxEditor {
    public partial class Plugin {
        public static void Draw() {
            if( Loading ) return;

            CopyManager.ResetAll();
            CheckWorkspaceKeybinds();

            TexToolsDialog.Draw();
            PenumbraDialog.Draw();
            ToolsDialog.Draw();
            VfxTracker.Draw();
            Configuration.Draw();

            Managers.ForEach( x => x?.Draw() );

            CopyManager.FinalizeAll();

            if( Configuration.AutosaveEnabled &&
                Configuration.AutosaveSeconds > 10 &&
                !string.IsNullOrEmpty( CurrentWorkspaceLocation ) &&
                ( DateTime.Now - LastAutoSave ).TotalSeconds > Configuration.AutosaveSeconds
            ) {
                LastAutoSave = DateTime.Now;
                SaveWorkspace();
            }
        }

        private static void CheckWorkspaceKeybinds() {
            if( Configuration.OpenKeybind.KeyPressed() ) OpenWorkspace();
            if( Configuration.SaveKeybind.KeyPressed() ) SaveWorkspace();
            if( Configuration.SaveAsKeybind.KeyPressed() ) SaveAsWorkspace();
        }

        public static void DrawFileMenu() {
            using var id = ImRaii.PushId( "##Menu" );
            if( ImGui.BeginMenu( $"File" ) ) {
                ImGui.TextDisabled( "Workspace" );
                ImGui.SameLine();
                UiUtils.HelpMarker( "A workspace allows you to save multiple vfx replacements at the same time, as well as any imported textures or item renaming (such as particles or emitters)" );

                if( ImGui.MenuItem( $"New" ) ) NewWorkspace();
                if( ImGui.MenuItem( $"Open" ) ) OpenWorkspace();
                if( ImGui.MenuItem( $"Save" ) ) SaveWorkspace();
                if( ImGui.MenuItem( $"Save As" ) ) SaveAsWorkspace();

                ImGui.Separator();
                if( ImGui.MenuItem( $"Settings" ) ) Configuration.Show();
                if( ImGui.MenuItem( $"Tools" ) ) ToolsDialog.Show();
                if( ImGui.BeginMenu( $"Help" ) ) {
                    if( ImGui.MenuItem( $"Github" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor" );
                    if( ImGui.MenuItem( $"Report an Issue" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues" );
                    if( ImGui.MenuItem( $"Wiki" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );
                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }

            if( ImGui.BeginMenu( $"Export" ) ) {
                if( ImGui.MenuItem( $"Penumbra" ) ) PenumbraDialog.Show();
                if( ImGui.MenuItem( $"TexTools" ) ) TexToolsDialog.Show();
                ImGui.EndMenu();
            }
        }

        public static void DrawManagersMenu( IFileManager manager ) {
            using var id = ImRaii.PushId( "##Menu" );
            if( ImGui.MenuItem( $"Textures" ) ) TextureManager.Show();
            ImGui.Separator();
            DrawManagerMenu( manager, $"Vfx", AvfxManager );
            DrawManagerMenu( manager, $"Tmb", TmbManager );
            DrawManagerMenu( manager, $"Pap", PapManager );
            DrawManagerMenu( manager, $"Scd", ScdManager );
            DrawManagerMenu( manager, $"Eid", EidManager );
            DrawManagerMenu( manager, $"Uld", UldManager );
        }

        private static void DrawManagerMenu( IFileManager manager, string text, IFileManager menuManager ) {
            var disabled = manager == menuManager;
            if( disabled ) ImGui.BeginDisabled();
            if( ImGui.MenuItem( text ) ) menuManager.Show();
            if( disabled ) ImGui.EndDisabled();
        }
    }
}