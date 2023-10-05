using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ImGuiNET;
using OtterGui;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using VfxEditor.Data;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor {
    public unsafe partial class Plugin {
        public static bool InGpose => Dalamud.ClientState.IsGPosing;
        public static GameObject GposeTarget => Dalamud.Objects.CreateObjectReference( new IntPtr( TargetSystem.Instance()->GPoseTarget ) );
        public static GameObject PlayerObject => InGpose ? GposeTarget : Dalamud.ClientState?.LocalPlayer;
        public static GameObject TargetObject => InGpose ? GposeTarget : Dalamud.TargetManager?.Target;

        private static readonly List<string> ModalsToOpen = new();
        public static readonly Dictionary<string, Modal> Modals = new();

        public static void Draw() {
            if( CheckLoadState() ) return;

            CopyManager.ResetAll();
            CheckWorkspaceKeybinds();

            TexToolsDialog.Draw();
            PenumbraDialog.Draw();
            ToolsDialog.Draw();
            TrackerManager.Draw();
            Configuration.Draw();
            LibraryManager.Draw();

            Managers.ForEach( x => x?.Draw() );

            CopyManager.FinalizeAll();

            if( Configuration.AutosaveEnabled && Configuration.AutosaveSeconds > 10 && !string.IsNullOrEmpty( CurrentWorkspaceLocation ) &&
                ( DateTime.Now - LastAutoSave ).TotalSeconds > Configuration.AutosaveSeconds ) {
                LastAutoSave = DateTime.Now;
                SaveWorkspace();
            }

            if( ModalsToOpen.Count > 0 ) {
                foreach( var title in ModalsToOpen ) ImGui.OpenPopup( title );
                ModalsToOpen.Clear();
            }
            foreach( var modal in Modals ) modal.Value.Draw();
        }

        private static void CheckWorkspaceKeybinds() {
            if( Configuration.OpenKeybind.KeyPressed() ) OpenWorkspace();
            if( Configuration.SaveKeybind.KeyPressed() ) SaveWorkspace();
            if( Configuration.SaveAsKeybind.KeyPressed() ) SaveAsWorkspace();
        }

        public static void DrawFileMenu() {
            using var _ = ImRaii.PushId( "Menu" );

            if( ImGui.BeginMenu( "File" ) ) {
                if( ImGui.MenuItem( "New" ) ) NewWorkspace();
                if( ImGui.MenuItem( "Open" ) ) OpenWorkspace();
                if( ImGui.BeginMenu( "Open Recent" ) ) {
                    foreach( var (recent, idx) in Configuration.RecentWorkspaces.WithIndex() ) {
                        if( ImGui.MenuItem( $"{recent.Item1}##{idx}" ) ) {
                            OpenWorkspaceAsync( recent.Item2 );
                            break;
                        }
                    }
                    ImGui.EndMenu();
                }
                if( ImGui.MenuItem( "Save" ) ) SaveWorkspace();
                if( ImGui.MenuItem( "Save As" ) ) SaveAsWorkspace();

                ImGui.Separator();
                if( ImGui.MenuItem( "Settings" ) ) Configuration.Show();
                if( ImGui.MenuItem( "Tools" ) ) ToolsDialog.Show();
                if( ImGui.BeginMenu( "Help" ) ) {
                    if( ImGui.MenuItem( "Github" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor" );
                    if( ImGui.MenuItem( "Report an Issue" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues" );
                    if( ImGui.MenuItem( "Wiki" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );
                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }

            if( ImGui.BeginMenu( "Export" ) ) {
                if( ImGui.MenuItem( "Penumbra" ) ) PenumbraDialog.Show();
                if( ImGui.MenuItem( "TexTools" ) ) TexToolsDialog.Show();
                ImGui.EndMenu();
            }
        }

        public static void DrawManagersMenu( IFileManager currentManager ) {
            using var _ = ImRaii.PushId( "Menu" );

            if( ImGui.MenuItem( "Textures" ) ) TextureManager.Show();
            ImGui.Separator();

            // Manually specify the order since it's different than the load order
            var managers = new IFileManager[] {
                AvfxManager,
                TmbManager,
                PapManager,
                ScdManager,
                UldManager,
                SklbManager,
                SkpManager,
                PhybManager,
                EidManager,
                AtchManager,
                ShpkManager,
            };

            var dropdown = false;

            for( var i = 0; i < managers.Length; i++ ) {
                var manager = managers[i];

                if( !dropdown && i < ( managers.Length - 1 ) ) { // no need for a dropdown for the last one
                    var width = ImGui.CalcTextSize( manager.GetId() ).X + ( 2 * ImGui.GetStyle().ItemSpacing.X ) + 10;

                    if( width > ImGui.GetContentRegionAvail().X ) {
                        dropdown = true;
                        using var font = ImRaii.PushFont( UiBuilder.IconFont );
                        if( !ImGui.BeginMenu( FontAwesomeIcon.CaretDown.ToIconString() ) ) return; // Menu hidden, just skip the rest
                    }
                }

                using var disabled = ImRaii.Disabled( manager == currentManager );
                if( ImGui.MenuItem( manager.GetId() ) ) manager.Show();
            }

            if( dropdown ) ImGui.EndMenu();
        }

        public static void AddModal( Modal modal ) {
            Modals[modal.Title] = modal;
            ModalsToOpen.Add( modal.Title ); // To eliminate Imgui ID weirdness
        }
    }
}