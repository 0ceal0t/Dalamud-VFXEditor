using Dalamud.Logging;
using ImGuiFileDialog;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using VFXEditor.AVFX;
using VFXEditor.PAP;
using VFXEditor.Texture;
using VFXEditor.TMB;

using VFXSelect;

namespace VFXEditor {
    public struct WorkspaceMeta {
        public WorkspaceMetaTex[] Tex;
        public WorkspaceMetaAvfx[] Docs;
        public WorkspaceMetaTmb[] Tmb;
        public WorkspaceMetaPap[] Pap;
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

    public partial class Plugin {
        public static string CurrentWorkspaceLocation { get; private set; } = "";
        private static DateTime lastAutosave = DateTime.Now;
        private static bool IsLoading = false;

        private static void NewWorkspace() {
            Task.Run( async () => {
                IsLoading = true;
                CurrentWorkspaceLocation = "";

                ResetTextureManager();
                ResetAvfxManager();
                ResetTmbManager();
                ResetPapManager();

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
                    PluginLog.Error( "Could not load workspace", e );
                }
            } );
        }

        private static void OpenWorkspaceFolder( string loadLocation ) {
            lastAutosave = DateTime.Now;
            var metaPath = Path.Combine( loadLocation, "vfx_workspace.json" );
            if( !File.Exists( metaPath ) ) {
                PluginLog.Error( "vfx_workspace.json does not exist" );
                return;
            }
            var meta = JsonConvert.DeserializeObject<WorkspaceMeta>( File.ReadAllText( metaPath ) );

            IsLoading = true;

            ResetTextureManager();
            var texRootPath = Path.Combine( loadLocation, TextureManager.PenumbraPath );
            if( meta.Tex != null ) {
                foreach( var tex in meta.Tex ) {
                    var fullPath = Path.Combine( texRootPath, tex.RelativeLocation );
                    TextureManager.ImportTexture( fullPath, tex.ReplacePath, tex.Height, tex.Width, tex.Depth, tex.MipLevels, tex.Format );
                }
            }

            ResetAvfxManager();
            var vfxRootPath = Path.Combine( loadLocation, AVFXManager.PenumbraPath );
            if( meta.Docs != null ) {
                foreach( var doc in meta.Docs ) {
                    var fullPath = ( doc.RelativeLocation == "" ) ? "" : Path.Combine( vfxRootPath, doc.RelativeLocation );
                    AvfxManager.ImportWorkspaceFile( fullPath, doc );
                }
            }

            ResetTmbManager();
            var tmbRootPath = Path.Combine( loadLocation, TMBManager.PenumbraPath );
            if( meta.Tmb != null ) {
                foreach( var tmb in meta.Tmb ) {
                    TmbManager.ImportWorkspaceFile( Path.Combine( tmbRootPath, tmb.RelativeLocation ), tmb );
                }
            }

            ResetPapManager();
            var papRootPath = Path.Combine( loadLocation, PAPManager.PenumbraPath );
            if( meta.Pap != null ) {
                foreach( var pap in meta.Pap ) {
                    PapManager.ImportWorkspaceFile( Path.Combine( papRootPath, pap.RelativeLocation ), pap );
                }
            }

            IsLoading = false;
        }

        private static void SaveWorkspace() {
            if( string.IsNullOrEmpty( CurrentWorkspaceLocation ) ) {
                SaveAsWorkspace();
            }
            else {
                Task.Run( async () => {
                    ExportWorkspace();
                } );
            }
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
            };

            var metaPath = Path.Combine( saveLocation, "vfx_workspace.json" );
            var metaString = JsonConvert.SerializeObject( meta );
            File.WriteAllText( metaPath, metaString );

            if( File.Exists( CurrentWorkspaceLocation ) ) File.Delete( CurrentWorkspaceLocation );
            ZipFile.CreateFromDirectory( saveLocation, CurrentWorkspaceLocation );
            Directory.Delete( saveLocation, true );
        }

        private static void ResetTextureManager() {
            var oldManager = TextureManager;
            TextureManager = new TextureManager();
            TextureManager.SetVisible( oldManager.IsVisible );
            oldManager?.Dispose();
        }

        private static void ResetAvfxManager() {
            var oldManager = AvfxManager;
            AvfxManager = new AVFXManager();
            AvfxManager.SetVisible( oldManager.IsVisible );
            oldManager?.Dispose();
        }

        private static void ResetTmbManager() {
            var oldManager = TmbManager;
            TmbManager = new TMBManager();
            TmbManager.SetVisible( oldManager.IsVisible );
            oldManager?.Dispose();
        }

        private static void ResetPapManager() {
            var oldManager = PapManager;
            PapManager = new PAPManager();
            PapManager.SetVisible( oldManager.IsVisible );
            oldManager?.Dispose();
        }
    }
}
