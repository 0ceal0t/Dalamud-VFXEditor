using Dalamud.Configuration;
using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Ui;
using VfxEditor.NodeLibrary;

namespace VfxEditor {
    [Serializable]
    public class Configuration : GenericDialog, IPluginConfiguration {
        public int Version { get; set; } = 0;
        public bool IsEnabled { get; set; } = true;

        public bool LogAllFiles = false;
        public bool LogDebug = false;
        public bool LogVfxDebug = false;

        public bool HideWithUI = true;
        public int SaveRecentLimit = 10;
        public bool OverlayLimit = true;
        public float OverlayRemoveDelay = 1;
        public string WriteLocation = Path.Combine( new[] {
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "XIVLauncher",
            "pluginConfigs",
            "VFXEditor",
        } );

        public List<SelectResult> RecentSelects = new();
        public List<SelectResult> RecentSelectsTMB = new();
        public List<SelectResult> RecentSelectsPAP = new();
        public List<SelectResult> RecentSelectsScd = new();

        public List<SelectResult> FavoriteVfx = new();
        public List<SelectResult> FavoriteTmb = new();
        public List<SelectResult> FavoritePap = new();
        public List<SelectResult> FavoriteScd = new();

        public bool FilepickerImagePreview = true;

        public bool AutosaveEnabled = false;
        public int AutosaveSeconds = 300;

        public bool BlockGameInputsWhenFocused = false;

        public KeybindConfiguration SaveKeybind = new();
        public KeybindConfiguration SaveAsKeybind = new();
        public KeybindConfiguration OpenKeybind = new();
        public KeybindConfiguration DocumentsKeybind = new();
        public KeybindConfiguration UpdateKeybind = new();
        public KeybindConfiguration CopyKeybind = new();
        public KeybindConfiguration PasteKeybind = new();
        public KeybindConfiguration UndoKeybind = new();
        public KeybindConfiguration RedoKeybind = new();
        public KeybindConfiguration SpawnOnSelfKeybind = new();
        public KeybindConfiguration SpawnOnGroundKeybind = new();
        public KeybindConfiguration SpawnOnTargetKeybind = new();

        public List<AvfxNodeLibraryItem> VFXNodeLibraryItems = new();

        public int MaxUndoSize = 10;
        public bool DoubleClickNavigate = true;

        public bool LoopMusic = true;
        public bool LoopSoundEffects = false;
        public float ScdVolume = 1f;

        public Configuration() : base( "Settings", false, 300, 200 ) { }

        public void Setup() {
            Plugin.PluginInterface.UiBuilder.DisableUserUiHide = !HideWithUI;
            FileDialogManager.ImagePreview = FilepickerImagePreview;
            PluginLog.Log( "Write location: " + WriteLocation );
        }

        public void AddRecent( List<SelectResult> recentList, SelectResult result ) {
            if( recentList.Contains( result ) ) {
                recentList.Remove( result ); // want to move it to the top
            }
            recentList.Add( result );
            if( recentList.Count > SaveRecentLimit ) {
                recentList.RemoveRange( 0, recentList.Count - SaveRecentLimit );
            }
            Save();
        }

        public void Save() {
            Plugin.PluginInterface.SavePluginConfig( this );
            Plugin.PluginInterface.UiBuilder.DisableUserUiHide = !HideWithUI;
        }

        public override void DrawBody() {
            var id = "##Settings";
            if( ImGui.BeginTabBar( $"{id}-TabBar" ) ) {
                if( ImGui.BeginTabItem( $"Configuration{id}" ) ) {
                    DrawConfiguration();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Keybinds{id}" ) ) {
                    DrawKeybinds();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawConfiguration() {
            ImGui.BeginChild( "##Settings-Config" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Indent( 5 );

            ImGui.TextDisabled( "Changes to the temp file location may require a restart to take effect" );
            if( ImGui.InputText( "Temp file location", ref WriteLocation, 255 ) ) Save();
            if( ImGui.Checkbox( "Log all files##Settings", ref LogAllFiles ) ) Save();
            if( ImGui.Checkbox( "Log debug information##Settings", ref LogDebug ) ) Save();
            if( ImGui.Checkbox( "Log Vfx debug information##Settings", ref LogVfxDebug ) ) Save();

            if( ImGui.Checkbox( "Autosave Workspace##Settings", ref AutosaveEnabled ) ) Save();
            ImGui.Indent();
            if( !AutosaveEnabled ) {
                var style = ImGui.GetStyle();
                ImGui.PushStyleVar( ImGuiStyleVar.Alpha, style.Alpha * 0.5f );
            }
            ImGui.SetNextItemWidth( 120 );
            if( ImGui.InputInt( "Autosave time (seconds)##Settings", ref AutosaveSeconds ) ) Save();
            if( !AutosaveEnabled ) ImGui.PopStyleVar();
            ImGui.Unindent();

            if( ImGui.Checkbox( "Hide with UI##Settings", ref HideWithUI ) ) Save();

            if( ImGui.Checkbox( "File picker image preview##Settings", ref FilepickerImagePreview ) ) {
                FileDialogManager.ImagePreview = FilepickerImagePreview;
                Save();
            }

            ImGui.SetNextItemWidth( 135 );
            if( ImGui.InputInt( "Recent file limit##Settings", ref SaveRecentLimit ) ) {
                SaveRecentLimit = Math.Max( SaveRecentLimit, 0 );
                Save();
            }

            ImGui.SetNextItemWidth( 135 );
            if( ImGui.InputFloat( "Live overlay remove delay time##Settings", ref OverlayRemoveDelay ) ) Save();
            if( ImGui.Checkbox( "Live overlay limit by distance##Settings", ref OverlayLimit ) ) Save();

            ImGui.SetNextItemWidth( 135 );
            if( ImGui.InputInt( "Undo history size", ref MaxUndoSize ) ) Save();

            if( ImGui.Checkbox( "Double-click to navigate to items", ref DoubleClickNavigate ) ) Save();

            ImGui.Unindent();
            ImGui.EndChild();
        }

        private void DrawKeybinds() {
            if( ImGui.Checkbox( "Block game inputs when VFXEditor is focused##Settings", ref BlockGameInputsWhenFocused ) ) Save();

            ImGui.BeginChild( "##Settings-Keybinds", new Vector2( -1 ), true );

            if( SaveKeybind.Draw( "Save", "##SaveKeybind" ) ) Save();
            if( SaveAsKeybind.Draw( "Save as", "##SaveAsKeybind" ) ) Save();
            if( OpenKeybind.Draw( "Open", "##OpenKeybind" ) ) Save();
            if( CopyKeybind.Draw( "Copy", "##CopyKeybind" ) ) Save();
            if( PasteKeybind.Draw( "Paste ", "##PasteKeybind" ) ) Save();
            if( UndoKeybind.Draw( "Undo", "##UndoKeybind" ) ) Save();
            if( RedoKeybind.Draw( "Redo ", "##RedoKeybind" ) ) Save();
            if( DocumentsKeybind.Draw( "Documents", "##DocumentsKeybind" ) ) Save();
            if( UpdateKeybind.Draw( "Update", "##UpdateKeybind" ) ) Save();
            if( SpawnOnSelfKeybind.Draw( "Spawn on self (VFX only)", "##SpawnSelfKeybind" ) ) Save();
            if( SpawnOnGroundKeybind.Draw( "Spawn on ground (VFX only)", "##SpawnGroundKeybind" ) ) Save();
            if( SpawnOnTargetKeybind.Draw( "Spawn on target (VFX only)", "##SpawnTargetKeybind" ) ) Save();

            ImGui.EndChild();
        }
    }
}