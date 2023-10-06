using ImGuiFileDialog;
using ImGuiNET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor {
    public enum WorkspaceState {
        None,
        Loading,
        Cleanup
    }

    public partial class Plugin {
        public static string CurrentWorkspaceLocation { get; private set; } = "";
        public static string CurrentWorkspaceName => string.IsNullOrEmpty( CurrentWorkspaceLocation ) ? "" : Path.GetFileName( CurrentWorkspaceLocation );
        public static WorkspaceState State { get; private set; } = WorkspaceState.None;

        // Havok init and texture wrap dispose
        public static Action OnMainThread;

        private static int WorkspaceFileCount = 0;
        private static DateTime LastAutoSave = DateTime.Now;

        // Return true if it's loading and the UI needs to be hidden
        private static bool CheckLoadState() {
            if( State == WorkspaceState.Cleanup ) {
                OnMainThread?.Invoke();
                OnMainThread = null;

                LastAutoSave = DateTime.Now;
                State = WorkspaceState.None;
                return true;
            }

            if( State != WorkspaceState.None ) {
                DrawLoadingDialog();
                return true;
            }

            return false;
        }

        private static async void NewWorkspace() {
            State = WorkspaceState.Loading;
            await Task.Run( async () => {
                await Task.Delay( 100 );
                WorkspaceFileCount = Managers.Count - 1;
                foreach( var manager in Managers.Where( x => x != null ) ) { manager.ToDefault(); }
                CurrentWorkspaceLocation = "";
                State = WorkspaceState.Cleanup;
            } );
        }

        private static void OpenWorkspace() {
            FileDialogManager.OpenFileDialog( "Select a Workspace File", "Workspace{.vfxworkspace,.json},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    var extension = new FileInfo( res ).Extension;
                    if( extension == ".json" ) { // OLD
                        var directory = Path.GetDirectoryName( res );
                        OpenWorkspaceAsync( directory.TrimEnd( Path.DirectorySeparatorChar ) + ".vfxworkspace", Path.GetDirectoryName( res ), false ); // Move to new format
                    }
                    else if( extension == ".vfxworkspace" ) { // NEW
                        OpenWorkspaceAsync( res );
                    }
                }
                catch( Exception e ) {
                    Dalamud.Error( e, "Could not load workspace" );
                }
            } );
        }

        private static void OpenWorkspaceAsync( string loadLocation ) => OpenWorkspaceAsync( loadLocation, Path.Combine( Path.GetDirectoryName( loadLocation ), "VFX_WORKSPACE_TEMP" ), true );

        private static async void OpenWorkspaceAsync( string loadLocation, string tempDir, bool unzip ) {
            State = WorkspaceState.Loading;
            await Task.Run( async () => {
                await Task.Delay( 100 );
                if( unzip ) ZipFile.ExtractToDirectory( loadLocation, tempDir, true );

                if( OpenWorkspaceFolder( tempDir ) ) {
                    CurrentWorkspaceLocation = loadLocation;
                    Configuration.AddRecentWorkspace( loadLocation );
                    State = WorkspaceState.Cleanup;
                }
                else {
                    State = WorkspaceState.None;
                }

                if( unzip ) Directory.Delete( tempDir, true );
            } );
        }

        private static bool OpenWorkspaceFolder( string loadLocation ) {
            WorkspaceFileCount = Directory.GetFiles( loadLocation, "*.*", SearchOption.AllDirectories ).Length;
            Dalamud.Log( $"Loading {WorkspaceFileCount} files from {loadLocation}" );

            var metaPath = Path.Combine( loadLocation, "vfx_workspace.json" );
            if( !File.Exists( metaPath ) ) {
                Dalamud.Error( "vfx_workspace.json does not exist" );
                return false;
            }

            var meta = JObject.Parse( File.ReadAllText( metaPath ) );
            foreach( var manager in Managers.Where( x => x != null ) ) {
                manager.Dispose();
                manager.WorkspaceImport( meta, loadLocation );
            }

            return true;
        }

        private static void DrawLoadingDialog() {
            if( ImGui.Begin( "Loading Workspace...", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking ) ) {
                var count = WorkspaceFileCount == 0 ? 0 : Managers.Where( x => x != null ).Select( x => x.GetDocuments().Count() ).Sum();
                ImGui.Text( $"{count} / {WorkspaceFileCount}" );

                var pos = ImGui.GetCursorScreenPos();
                var width = 300f;
                var height = 20f;
                ImGui.Dummy( new( width, height ) );

                var filled = Math.Min( width, WorkspaceFileCount == 0 ? 0 : ( ( float )count / WorkspaceFileCount ) * width );

                var drawList = ImGui.GetWindowDrawList();
                drawList.AddRectFilled( pos, pos + new Vector2( width, height ), ImGui.GetColorU32( ImGuiCol.FrameBg ), 5f );
                if( count > 0 ) {
                    drawList.AddRectFilled( pos, pos + new Vector2( filled, height ), ImGui.ColorConvertFloat4ToU32( UiUtils.PARSED_GREEN ), 5f );
                }

                ImGui.End();
            }
        }

        // =================================

        private static void SaveWorkspace() {
            if( string.IsNullOrEmpty( CurrentWorkspaceLocation ) ) {
                SaveAsWorkspace();
            }
            else {
                ExportWorkspace();
            }
        }

        private static void SaveAsWorkspace() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".vfxworkspace", "workspace", "vfxworkspace", ( bool ok, string res ) => {
                if( !ok ) return;
                CurrentWorkspaceLocation = res;
                ExportWorkspace();
            } );
        }

        private static async void ExportWorkspace() {
            await Task.Run( () => {
                var saveLocation = Path.Combine( Path.GetDirectoryName( CurrentWorkspaceLocation ), "VFX_WORKSPACE_TEMP" );
                Directory.CreateDirectory( saveLocation );

                var meta = new Dictionary<string, string>();
                Managers.ForEach( x => x?.WorkspaceExport( meta, saveLocation ) );

                var metaPath = Path.Combine( saveLocation, "vfx_workspace.json" );
                var metaString = JsonConvert.SerializeObject( meta );
                File.WriteAllText( metaPath, metaString );

                if( File.Exists( CurrentWorkspaceLocation ) ) File.Delete( CurrentWorkspaceLocation );
                ZipFile.CreateFromDirectory( saveLocation, CurrentWorkspaceLocation );
                Directory.Delete( saveLocation, true );

                UiUtils.OkNotification( "Saved workspace" );
            } );
        }

        public static void CleanupExport( IFileDocument document ) {
            TexToolsDialog.RemoveDocument( document );
            PenumbraDialog.RemoveDocument( document );
        }
    }
}
