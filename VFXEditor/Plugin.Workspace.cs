using ImGuiNET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using VfxEditor.FileBrowser;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Ui.Export;
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
        public static bool Saving { get; private set; } = false;

        private static readonly SemaphoreSlim SavingLock = new( 1, 1 );
        private static int BackupId = 0;

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

        private static void CheckAutoSave() {
            if( !Configuration.AutosaveEnabled || Configuration.AutosaveSeconds < 10 ) return;
            if( ( DateTime.Now - LastAutoSave ).TotalSeconds <= Configuration.AutosaveSeconds ) return;

            LastAutoSave = DateTime.Now;

            // Try the next time
            if( Saving ) return;
            if( string.IsNullOrEmpty( CurrentWorkspaceLocation ) ) return;

            Dalamud.Log( "Autosaving workspace..." );
            if( Configuration.AutosaveBackups ) {
                var id = BackupId++ % Configuration.BackupCount;
                ExportWorkspace( CurrentWorkspaceLocation.Replace( ".vfxworkspace", $" - {id}.vfxworkspace" ), false );
            }
            else {
                ExportWorkspace(); // Overwrite current file
            }
        }

        private static async void NewWorkspace() {
            State = WorkspaceState.Loading;
            await Task.Run( async () => {
                await Task.Delay( 100 );
                WorkspaceFileCount = Managers.Count - 1;
                foreach( var manager in Managers.Where( x => x != null ) ) { manager.Reset( ResetType.ToDefault ); }
                FileBrowserManager.Dispose();
                ExportDialog.Reset();

                CurrentWorkspaceLocation = "";
                State = WorkspaceState.Cleanup;
            } );
        }

        private static void OpenWorkspace( bool reset ) {
            FileBrowserManager.OpenFileDialog( "Select a Workspace File", "Workspace{.vfxworkspace,.json},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    var extension = new FileInfo( res ).Extension;
                    if( extension == ".json" ) { // OLD
                        var directory = Path.GetDirectoryName( res );
                        OpenWorkspaceAsync( directory.TrimEnd( Path.DirectorySeparatorChar ) + ".vfxworkspace", Path.GetDirectoryName( res ), false, reset ); // Move to new format
                    }
                    else if( extension == ".vfxworkspace" ) { // NEW
                        OpenWorkspaceAsync( res, reset );
                    }
                }
                catch( Exception e ) {
                    Dalamud.Error( e, "Could not load workspace" );
                }
            } );
        }

        private static void OpenWorkspaceAsync( string loadLocation, bool reset ) => OpenWorkspaceAsync( loadLocation, Path.Combine( Path.GetDirectoryName( loadLocation ), "VFX_WORKSPACE_IN" ), true, reset );

        private static void OpenWorkspaceAsync( string loadLocation, string tempDir, bool unzip, bool reset ) {
            State = WorkspaceState.Loading;
            Task.Run( async () => {
                await Task.Delay( 100 );
                if( unzip ) ZipFile.ExtractToDirectory( loadLocation, tempDir, true );

                if( OpenWorkspaceFolder( tempDir, reset ) ) {
                    CurrentWorkspaceLocation = loadLocation;
                    BackupId = 0;
                    Configuration.AddRecentWorkspace( loadLocation );
                    State = WorkspaceState.Cleanup;
                }
                else {
                    State = WorkspaceState.None;
                }

                if( unzip ) Directory.Delete( tempDir, true );
            } );
        }

        private static bool OpenWorkspaceFolder( string loadLocation, bool reset ) {
            WorkspaceFileCount = Directory.GetFiles( loadLocation, "*.*", SearchOption.AllDirectories ).Length;
            Dalamud.Log( $"Loading {WorkspaceFileCount} files from {loadLocation}" );

            var metaPath = Path.Combine( loadLocation, "vfx_workspace.json" );
            if( !File.Exists( metaPath ) ) {
                Dalamud.Error( "vfx_workspace.json does not exist" );
                return false;
            }

            var offsets = new Dictionary<IFileManager, int>(); // Number of documents before import
            var meta = JObject.Parse( File.ReadAllText( metaPath ) );
            foreach( var manager in Managers.Where( x => x != null ) ) {
                if( reset ) manager.Reset( ResetType.Reset );
                offsets[manager] = manager.GetDocuments().Count();
                manager.WorkspaceImport( meta, loadLocation );
            }

            if( reset ) ExportDialog.Reset();
            PenumbraDialog.WorkspaceImport( meta, offsets );

            FileBrowserManager.Dispose();

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
            if( string.IsNullOrEmpty( CurrentWorkspaceLocation ) ) SaveAsWorkspace();
            else ExportWorkspace();
        }

        private static void SaveAsWorkspace() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".vfxworkspace", "workspace", "vfxworkspace", ( bool ok, string res ) => {
                if( !ok ) return;
                ExportWorkspace( res );
                Configuration.AddRecentWorkspace( res );
            } );
        }

        private static void ExportWorkspace( string location = null, bool updateLocation = true ) {
            Task.Run( async () => {
                if( !await SavingLock.WaitAsync( 1000 ) ) {
                    Dalamud.Error( "Could not get lock" );
                    return;
                }
                Saving = true;

                try {
                    var workspaceLocation = string.IsNullOrEmpty( location ) ? CurrentWorkspaceLocation : location;

                    var saveLocation = Path.Combine( Path.GetDirectoryName( workspaceLocation ), "VFX_WORKSPACE_OUT" );
                    Directory.CreateDirectory( saveLocation );

                    var meta = new Dictionary<string, string>();
                    Managers.ForEach( x => x?.WorkspaceExport( meta, saveLocation ) );
                    PenumbraDialog.WorkspaceExport( meta );

                    var metaPath = Path.Combine( saveLocation, "vfx_workspace.json" );
                    var metaString = JsonConvert.SerializeObject( meta );
                    File.WriteAllText( metaPath, metaString );

                    if( File.Exists( workspaceLocation ) ) File.Delete( workspaceLocation );
                    ZipFile.CreateFromDirectory( saveLocation, workspaceLocation );
                    Dalamud.Log( $"Saved to {workspaceLocation}" );
                    Directory.Delete( saveLocation, true );

                    if( updateLocation ) {
                        CurrentWorkspaceLocation = workspaceLocation;
                        BackupId = 0;
                    }
                }
                catch( Exception ex ) {
                    Dalamud.Error( ex, "Error saving workspace" );
                }

                SavingLock.Release(); // Make sure to release!
                Saving = false;
            } );
        }
    }
}
