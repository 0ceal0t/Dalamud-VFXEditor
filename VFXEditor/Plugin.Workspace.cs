using Dalamud.Logging;
using ImGuiFileDialog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Interop.Havok;
using VfxEditor.Utils;

namespace VfxEditor {
    public enum LoadState {
        None,
        Loading
    }

    public partial class Plugin {
        public static string CurrentWorkspaceLocation { get; private set; } = "";

        private static DateTime LastAutoSave = DateTime.Now;
        public static LoadState Loading { get; private set; } = LoadState.None;
        public static bool LoadAsync { get; private set; } = false;
        public static readonly List<HavokData> HavokToInit = new(); // Make sure this is initialized on the main thread

        private static async void NewWorkspace() {
            await Task.Run( () => {
                Loading = LoadState.Loading;
                CurrentWorkspaceLocation = "";
                Managers.ForEach( x => x?.ToDefault() );
                LastAutoSave = DateTime.Now;
                Loading = LoadState.None;
            } );
        }

        private static void OpenWorkspace() {
            FileDialogManager.OpenFileDialog( "Select a Workspace File", "Workspace{.vfxworkspace,.json},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    var file = new FileInfo( res );
                    var directory = Path.GetDirectoryName( res );

                    if( file.Extension == ".json" ) { // OLD
                        if( !OpenWorkspaceFolder( directory ) ) return;
                        CurrentWorkspaceLocation = directory.TrimEnd( Path.DirectorySeparatorChar ) + ".vfxworkspace"; // move to new format
                    }
                    else if( file.Extension == ".vfxworkspace" ) { // NEW
                        OpenWorkspaceAsync( res, Path.Combine( directory, "VFX_WORKSPACE_TEMP" ) );
                    }
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not load workspace" );
                }
            } );
        }

        private static async void OpenWorkspaceAsync( string loadLocation, string tempDir ) {
            await Task.Run( () => {
                ZipFile.ExtractToDirectory( loadLocation, tempDir, true );
                LoadAsync = true;
                OpenWorkspaceFolder( tempDir );
                LoadAsync = false;
                Directory.Delete( tempDir, true );
                CurrentWorkspaceLocation = loadLocation;
            } );
        }

        private static bool OpenWorkspaceFolder( string loadLocation ) {
            var metaPath = Path.Combine( loadLocation, "vfx_workspace.json" );
            if( !File.Exists( metaPath ) ) {
                PluginLog.Error( "vfx_workspace.json does not exist" );
                return false;
            }

            var meta = JObject.Parse( File.ReadAllText( metaPath ) );

            Loading = LoadState.Loading;

            foreach( var manager in Managers ) {
                if( manager == null ) continue;
                manager.Dispose();
                manager.WorkspaceImport( meta, loadLocation );
            }

            LastAutoSave = DateTime.Now;
            Loading = LoadState.None;

            return true;
        }

        private static async void SaveWorkspace() {
            if( string.IsNullOrEmpty( CurrentWorkspaceLocation ) ) SaveAsWorkspace();
            else await Task.Run( ExportWorkspace );
        }

        private static void SaveAsWorkspace() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".vfxworkspace", "workspace", "vfxworkspace", ( bool ok, string res ) => {
                if( !ok ) return;
                CurrentWorkspaceLocation = res;
                ExportWorkspace();
            } );
        }

        private static void ExportWorkspace() {
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
        }

        public static void CleanupExport( IFileDocument document ) {
            TexToolsDialog.RemoveDocument( document );
            PenumbraDialog.RemoveDocument( document );
        }
    }
}
