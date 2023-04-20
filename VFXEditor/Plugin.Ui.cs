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
            var id = "##Menu";
            if( ImGui.BeginMenu( $"File{id}" ) ) {
                ImGui.TextDisabled( "Workspace" );
                ImGui.SameLine();
                UiUtils.HelpMarker( "A workspace allows you to save multiple vfx replacements at the same time, as well as any imported textures or item renaming (such as particles or emitters)" );

                if( ImGui.MenuItem( $"New{id}" ) ) NewWorkspace();
                if( ImGui.MenuItem( $"Open{id}" ) ) OpenWorkspace();
                if( ImGui.MenuItem( $"Save{id}" ) ) SaveWorkspace();
                if( ImGui.MenuItem( $"Save As{id}" ) ) SaveAsWorkspace();

                ImGui.Separator();
                if( ImGui.MenuItem( $"Settings{id}" ) ) Configuration.Show();
                if( ImGui.MenuItem( $"Tools{id}" ) ) ToolsDialog.Show();
                if( ImGui.BeginMenu( $"Help{id}" ) ) {
                    if( ImGui.MenuItem( $"Github{id}" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor" );
                    if( ImGui.MenuItem( $"Report an Issue#{id}" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues" );
                    if( ImGui.MenuItem( $"Wiki{id}" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );
                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }

            if( ImGui.BeginMenu( $"Export{id}" ) ) {
                if( ImGui.MenuItem( $"Penumbra{id}" ) ) PenumbraDialog.Show();
                if( ImGui.MenuItem( $"TexTools{id}" ) ) TexToolsDialog.Show();
                ImGui.EndMenu();
            }
        }

        public static void DrawManagersMenu( IFileManager manager ) {
            var id = "##Menu";
            if( ImGui.MenuItem( $"Textures{id}" ) ) TextureManager.Show();
            ImGui.Separator();
            DrawManagerMenu( manager, $"Vfx{id}", AvfxManager );
            DrawManagerMenu( manager, $"Tmb{id}", TmbManager );
            DrawManagerMenu( manager, $"Pap{id}", PapManager );
            DrawManagerMenu( manager, $"Scd{id}", ScdManager );
            DrawManagerMenu( manager, $"Eid{id}", EidManager );
            DrawManagerMenu( manager, $"Uld{id}", UldManager );
        }

        private static void DrawManagerMenu( IFileManager manager, string text, IFileManager menuManager ) {
            var disabled = manager == menuManager;
            if( disabled ) ImGui.BeginDisabled();
            if( ImGui.MenuItem( text ) ) menuManager.Show();
            if( disabled ) ImGui.EndDisabled();
        }
    }
}