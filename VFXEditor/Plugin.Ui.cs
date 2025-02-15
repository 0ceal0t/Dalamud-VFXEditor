using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor {
    public unsafe partial class Plugin {
        public static bool InGpose => Dalamud.ClientState.IsGPosing;
        public static IGameObject GposeTarget => Dalamud.Objects.CreateObjectReference( new IntPtr( TargetSystem.Instance()->GPoseTarget ) );
        public static IGameObject PlayerObject => InGpose ? GposeTarget : Dalamud.ClientState?.LocalPlayer;
        public static IGameObject TargetObject => InGpose ? GposeTarget : Dalamud.TargetManager?.Target;

        private static readonly List<string> ModalsToOpen = [];
        public static readonly Dictionary<string, Modal> Modals = [];

        public static void Draw() {
            IsImguiSafe = true;
            if( CheckLoadState() ) return;

            CheckWorkspaceKeybinds();

            WindowSystem.Draw();
            TrackerManager.Draw();

            CheckAutoSave();

            if( ModalsToOpen.Count > 0 ) {
                foreach( var title in ModalsToOpen ) ImGui.OpenPopup( title );
                ModalsToOpen.Clear();
            }
            foreach( var modal in Modals ) modal.Value.Draw();
        }

        private static void CheckWorkspaceKeybinds() {
            if( Configuration.OpenKeybind.KeyPressed() ) OpenWorkspace( true );
            if( Configuration.SaveKeybind.KeyPressed() ) SaveWorkspace();
            if( Configuration.SaveAsKeybind.KeyPressed() ) SaveAsWorkspace();
        }

        public static void DrawFileMenu() {
            using var _ = ImRaii.PushId( "Menu" );

            if( ImGui.BeginMenu( "File" ) ) {
                if( ImGui.MenuItem( "New" ) ) NewWorkspace();
                if( ImGui.MenuItem( "Open" ) ) OpenWorkspace( true );
                if( ImGui.BeginMenu( "Open Recent" ) ) {
                    foreach( var (recent, idx) in Configuration.RecentWorkspaces.WithIndex() ) {
                        if( ImGui.MenuItem( $"{recent.Item1}##{idx}" ) ) {
                            if( File.Exists( recent.Item2 ) ) {
                                OpenWorkspaceAsync( recent.Item2, true );
                            }
                            else {
                                Dalamud.Error( $"{recent.Item2} does not exist" );
                            }
                            break;
                        }
                    }
                    ImGui.EndMenu();
                }
                if( ImGui.MenuItem( "Append" ) ) OpenWorkspace( false );
                if( ImGui.MenuItem( "Save" ) ) SaveWorkspace();
                if( ImGui.MenuItem( "Save As" ) ) SaveAsWorkspace();

                ImGui.Separator();
                if( ImGui.BeginMenu( "Penumbra" ) )
                {
                    if( ImGui.MenuItem( "Open Mod" ) ) OpenWorkspacePenumbra( true );
                    if( ImGui.MenuItem( "Append Mod" ) ) OpenWorkspacePenumbra( false );
                    ImGui.EndMenu();
                }

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

            // Manually specify the order since it's different than the load order
            var categories = new List<IFileManager[]> {
                new IFileManager[]{
                    AvfxManager,
                    TextureManager
                },
                new IFileManager[]{
                    TmbManager,
                    PapManager,
                },
                new IFileManager[]{
                    ScdManager
                },
                new IFileManager[]{
                    UldManager
                },
                new IFileManager[]{
                    SklbManager,
                    SkpManager,
                    PhybManager,
                    EidManager,
                    AtchManager,
                    KdbManager,
                    PbdManager,
                },
                new IFileManager[]{
                    MdlManager,
                    MtrlManager,
                    ShpkManager,
                    ShcdManager
                }
            };

            var dropdown = false;
            var managerCount = categories.Sum( x => x.Length );
            var managerIdx = 0;

            foreach( var (category, categoryIdx) in categories.WithIndex() ) {
                foreach( var manager in category ) {
                    if( !dropdown && managerIdx < ( managerCount - 1 ) ) { // no need for a dropdown for the last one
                        var width = ImGui.CalcTextSize( manager.GetName() ).X + ( 2 * ImGui.GetStyle().ItemSpacing.X ) + 10;

                        if( width > ImGui.GetContentRegionAvail().X ) {
                            dropdown = true;
                            using var font = ImRaii.PushFont( UiBuilder.IconFont );
                            if( !ImGui.BeginMenu( FontAwesomeIcon.EllipsisH.ToIconString() ) ) return; // Menu hidden, just skip the rest
                        }
                    }

                    using var disabled = ImRaii.Disabled( manager == currentManager );
                    if( ImGui.MenuItem( manager.GetName() ) ) manager.Show();

                    managerIdx++;
                }

                if( categoryIdx < categories.Count - 1 ) ImGui.Separator();
            }

            if( dropdown ) ImGui.EndMenu();
        }

        public static void AddModal( Modal modal ) {
            Modals[modal.Title] = modal;
            ModalsToOpen.Add( modal.Title ); // To eliminate Imgui ID weirdness
        }
    }
}