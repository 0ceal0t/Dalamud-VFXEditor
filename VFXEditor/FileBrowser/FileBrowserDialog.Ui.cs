using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.FileBrowser {
    public partial class FileBrowserDialog {
        private bool IsOk = false;
        private bool WantsToQuit = false;
        private bool WaitingForConfirmation;
        private bool DrawnBefore = false;

        public bool Draw() {
            if( !Visible ) return false;

            var done = false;
            var name = $"{Title}##{Id}";

            IsOk = false;
            WantsToQuit = false;

            using var _ = ImRaii.PushId( "FileBrowser" );
            ImGui.SetNextWindowSize( new Vector2( 1000, 700 ), ImGuiCond.FirstUseEver );
            if( Modal && !WaitingForConfirmation ) {
                ImGui.OpenPopup( name );
                if( ImGui.BeginPopupModal( name, ref Visible, ImGuiWindowFlags.NoScrollbar ) ) {
                    done = DrawContents();
                    ImGui.EndPopup();
                }
            }
            else {
                if( !DrawnBefore ) {
                    ImGui.SetNextWindowFocus();
                    ImGui.SetNextWindowCollapsed( false );
                    DrawnBefore = true;
                }
                if( ImGui.Begin( name, ref Visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoDocking ) ) {
                    done = DrawContents();
                    ImGui.End();
                }
            }

            return DrawOverwrite( done );
        }

        private bool DrawContents() {
            if( !Visible ) { // Closed, all done
                IsOk = false;
                return true;
            }

            Filters.CheckFilters();

            DrawHeader();
            DrawBody();
            return DrawFooter();
        }

        private bool DrawOverwrite( bool done ) {
            if( FolderDialog || !ConfirmOverwrite ) return done; // don't need to confirm
            if( !IsOk && done ) return true; // cancelled

            if( WaitingForConfirmation || ( IsOk && done ) ) { // waiting for confirmation, or just clicked "OK"
                if( IsOk ) {
                    if( !File.Exists( GetFullFilePath() ) ) {
                        return true; // file doesn't exist, so no need for comfirmation dialog
                    }
                    else {
                        IsOk = false;
                        WaitingForConfirmation = true;
                    }
                }

                var confirmed = false;
                var open = true;

                var name = "Overwrite This File?";
                ImGui.OpenPopup( name );
                if( ImGui.BeginPopupModal( name, ref open, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove ) ) {
                    if( ImGui.Checkbox( "Never ask me again", ref Plugin.Configuration.FileBrowserOverwriteDontAsk ) ) Plugin.Configuration.Save();

                    if( ImGui.Button( "OK", new Vector2( 120, 0 ) ) ) {
                        WaitingForConfirmation = false;
                        IsOk = true;
                        confirmed = true;
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.SameLine();
                    using var style = ImRaii.PushColor( ImGuiCol.Button, UiUtils.RED_COLOR );
                    if( ImGui.Button( "Cancel", new Vector2( 120, 0 ) ) ) {
                        WaitingForConfirmation = false;
                        IsOk = false;
                        confirmed = false;
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }

                return confirmed;
            }

            return false;
        }

        public string GetFullFilePath() {
            var path = GetCurrentPath();
            var fileName = GetCurrentFileName();
            return string.IsNullOrEmpty( fileName ) ? path : Path.Combine( path, fileName );
        }

        public string GetCurrentPath() {
            if( FolderDialog && !string.IsNullOrEmpty( FileNameInput ) && FileNameInput != "." ) { // combine path file with directory input
                return string.IsNullOrEmpty( CurrentPath ) ? FileNameInput : Path.Combine( CurrentPath, FileNameInput );
            }
            return CurrentPath;
        }
        public string GetCurrentFileName() => FolderDialog ? "" : Filters.ApplySelectedFilterExtension( FileNameInput );

        public bool GetIsOk() => IsOk;

        public string GetResult() {
            if( !SelectOnly ) return GetFullFilePath();
            if( FolderDialog && Selected == null ) {
                return GetFullFilePath(); // current directory
            }

            if( Selected == null ) return "";

            return Path.Combine( CurrentPath, Selected.FileName );
        }
    }
}
