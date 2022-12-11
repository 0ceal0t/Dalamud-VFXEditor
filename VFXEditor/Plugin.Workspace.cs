using Dalamud.Logging;
using ImGuiFileDialog;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using VfxEditor.AvfxFormat;
using VfxEditor.Utils;
using VfxEditor.PapFormat;
using VfxEditor.Texture;
using VfxEditor.TmbFormat;
using VfxEditor.ScdFormat;

namespace VfxEditor {
    public struct WorkspaceMeta {
        public WorkspaceMetaTex[] Tex;
        public WorkspaceMetaAvfx[] Docs;
        public WorkspaceMetaTmb[] Tmb;
        public WorkspaceMetaPap[] Pap;
        public WorkspaceMetaScd[] Scd;
    }

    public struct WorkspaceMetaAvfx {
        public SelectResult Source;
        public SelectResult Replace;
        public Dictionary<string, string> Renaming;

        public string RelativeLocation; // can be empty if no avfx file
    }

    public struct WorkspaceMetaTex {
        public int Height;
        public int Width;
        public int Depth;
        public int MipLevels;
        public TextureFormat Format;

        public string RelativeLocation;
        public string ReplacePath;
    }

    public struct WorkspaceMetaTmb {
        public SelectResult Source;
        public SelectResult Replace;
        public string RelativeLocation;
    }

    public struct WorkspaceMetaPap {
        public SelectResult Source;
        public SelectResult Replace;
        public string RelativeLocation;
    }

    public struct WorkspaceMetaScd {
        public SelectResult Source;
        public SelectResult Replace;
        public string RelativeLocation;
    }

    public partial class Plugin {
        public static string CurrentWorkspaceLocation { get; private set; } = "";

        private static DateTime LastAutoSave = DateTime.Now;
        private static bool IsLoading = false;

        private static async void NewWorkspace() {
            await Task.Run( () => {
                IsLoading = true;
                CurrentWorkspaceLocation = "";

                ResetTextureManager();
                ResetAvfxManager();
                ResetTmbManager();
                ResetPapManager();
                ResetScdManager();

                IsLoading = false;
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
            var meta = JsonConvert.DeserializeObject<WorkspaceMeta>( File.ReadAllText( metaPath ) );

            IsLoading = true;

            ResetTextureManager();
            if( meta.Tex != null ) {
                foreach( var tex in meta.Tex ) {
                    var fullPath = ResolveWorkspacePath( tex.RelativeLocation, loadLocation, TextureManager.PenumbraPath );
                    TextureManager.ImportTexture( fullPath, tex.ReplacePath, tex.Height, tex.Width, tex.Depth, tex.MipLevels, tex.Format );
                }
            }

            ResetAvfxManager();
            if( meta.Docs != null ) {
                foreach( var doc in meta.Docs ) AvfxManager.ImportWorkspaceFile( ResolveWorkspacePath( doc.RelativeLocation, loadLocation, AvfxManager.PenumbraPath ), doc );
            }

            ResetTmbManager();
            if( meta.Tmb != null ) {
                foreach( var tmb in meta.Tmb ) TmbManager.ImportWorkspaceFile( ResolveWorkspacePath( tmb.RelativeLocation, loadLocation, TmbManager.PenumbraPath ), tmb );
            }

            ResetPapManager();
            if( meta.Pap != null ) {
                foreach( var pap in meta.Pap ) PapManager.ImportWorkspaceFile( ResolveWorkspacePath( pap.RelativeLocation, loadLocation, PapManager.PenumbraPath ), pap );
            }

            ResetScdManager();
            if( meta.Scd != null ) {
                foreach( var scd in meta.Scd ) ScdManager.ImportWorkspaceFile( ResolveWorkspacePath( scd.RelativeLocation, loadLocation, ScdManager.PenumbraPath ), scd );
            }

            UiUtils.OkNotification( "Opened workspace" );

            IsLoading = false;
        }

        private static string ResolveWorkspacePath( string relativeLocation, string loadLocation, string penumbraPath ) => 
            ( relativeLocation == "" ) ? "" : Path.Combine( Path.Combine( loadLocation, penumbraPath ), relativeLocation );

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

            var meta = new WorkspaceMeta {
                Docs = AvfxManager.WorkspaceExport( saveLocation ),
                Tex = TextureManager.WorkspaceExport( saveLocation ),
                Tmb = TmbManager.WorkspaceExport( saveLocation ),
                Pap = PapManager.WorkspaceExport( saveLocation ),
                Scd = ScdManager.WorkspaceExport( saveLocation ),
            };

            var metaPath = Path.Combine( saveLocation, "vfx_workspace.json" );
            var metaString = JsonConvert.SerializeObject( meta );
            File.WriteAllText( metaPath, metaString );

            if( File.Exists( CurrentWorkspaceLocation ) ) File.Delete( CurrentWorkspaceLocation );
            ZipFile.CreateFromDirectory( saveLocation, CurrentWorkspaceLocation );
            Directory.Delete( saveLocation, true );

            UiUtils.OkNotification( "Saved workspace" );
        }

        private static void ResetTextureManager() {
            var oldManager = TextureManager;
            TextureManager = new TextureManager();
            TextureManager.SetVisible( oldManager.IsVisible );
            oldManager?.Dispose();
        }

        private static void ResetAvfxManager() {
            var oldManager = AvfxManager;
            AvfxManager = new AvfxManager();
            AvfxManager.SetVisible( oldManager.IsVisible );
            oldManager?.Dispose();
        }

        private static void ResetTmbManager() {
            var oldManager = TmbManager;
            TmbManager = new TmbManager();
            TmbManager.SetVisible( oldManager.IsVisible );
            oldManager?.Dispose();
        }

        private static void ResetPapManager() {
            var oldManager = PapManager;
            PapManager = new PapManager();
            PapManager.SetVisible( oldManager.IsVisible );
            oldManager?.Dispose();
        }

        private static void ResetScdManager() {
            var oldManager = ScdManager;
            ScdManager = new ScdManager();
            ScdManager.SetVisible( oldManager.IsVisible );
            oldManager?.Dispose();
        }
    }
}
