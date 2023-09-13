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
using VfxEditor.Utils;

namespace VfxEditor {
    public partial class Plugin {
        public static string CurrentWorkspaceLocation { get; private set; } = "";

        private static DateTime LastAutoSave = DateTime.Now;
        private static bool Loading = false;

        private static async void NewWorkspace() {
            await Task.Run( () => {
                Loading = true;
                CurrentWorkspaceLocation = "";
                Managers.ForEach( x => x?.ToDefault() );
                Loading = false;
            } );
        }

        private static void OpenWorkspace() {
            FileDialogManager.OpenFileDialog( "Select a Workspace File", "Workspace{.vfxworkspace,.json}", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    var selectedFile = new FileInfo( res );
                    var dir = Path.GetDirectoryName( res );

                    if( selectedFile.Extension == ".json" ) { // OLD
                        OpenWorkspaceFolder( dir );
                        CurrentWorkspaceLocation = dir.TrimEnd( Path.DirectorySeparatorChar ) + ".vfxworkspace"; // move to new format
                    }
                    else if( selectedFile.Extension == ".vfxworkspace" ) { // NEW
                        var tempDir = Path.Combine( dir, "VFX_WORKSPACE_TEMP" );
                        ZipFile.ExtractToDirectory( res, tempDir, true );
                        OpenWorkspaceFolder( tempDir );
                        Directory.Delete( tempDir, true );
                        CurrentWorkspaceLocation = res;
                    }
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not load workspace" );
                }
            } );
        }

        private static void OpenWorkspaceFolder( string loadLocation ) {
            LastAutoSave = DateTime.Now;
            var metaPath = Path.Combine( loadLocation, "vfx_workspace.json" );
            if( !File.Exists( metaPath ) ) {
                PluginLog.Error( "vfx_workspace.json does not exist" );
                return;
            }

            var meta = JObject.Parse( File.ReadAllText( metaPath ) );

            Loading = true;

            foreach( var manager in Managers ) {
                if( manager == null ) continue;
                manager.Dispose(); // clean up
                manager.WorkspaceImport( meta, loadLocation );
            }

            UiUtils.OkNotification( "Opened workspace" );

            Loading = false;
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
