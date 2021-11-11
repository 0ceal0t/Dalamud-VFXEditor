using Dalamud.Logging;
using ImGuiFileDialog;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using VFXEditor.Document;
using VFXEditor.Texture;
using VFXEditor.Tmb;
using VFXSelect.UI;

namespace VFXEditor {
    public struct WorkspaceMeta {
        public WorkspaceMetaTex[] Tex;
        public WorkspaceMetaDocument[] Docs;
        public WorkspaceMetaTmb[] Tmb;
    }

    public struct WorkspaceMetaDocument {
        public VFXSelectResult Source;
        public VFXSelectResult Replace;
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
        public VFXSelectResult Source; // Not used right now
        public VFXSelectResult Replace; // Not used right now

        public string RelativeLocation;
        public string ReplacePath;
    }

    public partial class Plugin {
        private string CurrentWorkspaceLocation = "";

        private void NewWorkspace() {
            Task.Run( async () => {
                IsLoading = true;
                CurrentWorkspaceLocation = "";

                ResetTextureManager();
                ResetDocumentManager();
                ResetTmbManager();

                IsLoading = false;
            } );
        }

        private void OpenWorkspace() {
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

        private void OpenWorkspaceFolder( string loadLocation ) {
            var metaPath = Path.Combine( loadLocation, "vfx_workspace.json" );
            if( !File.Exists( metaPath ) ) {
                PluginLog.Error( "vfx_workspace.json does not exist" );
                return;
            }
            var meta = JsonConvert.DeserializeObject<WorkspaceMeta>( File.ReadAllText( metaPath ) );

            IsLoading = true;

            ResetTextureManager();

            var texRootPath = Path.Combine( loadLocation, "Tex" );
            if( meta.Tex != null ) {
                foreach( var tex in meta.Tex ) {
                    var fullPath = Path.Combine( texRootPath, tex.RelativeLocation );
                    TextureManager.AddReplaceTexture( fullPath, tex.ReplacePath, tex.Height, tex.Width, tex.Depth, tex.MipLevels, tex.Format );
                }
            }

            ResetDocumentManager();

            var vfxRootPath = Path.Combine( loadLocation, "VFX" );
            if( meta.Docs != null ) {
                foreach( var doc in meta.Docs ) {
                    var fullPath = ( doc.RelativeLocation == "" ) ? "" : Path.Combine( vfxRootPath, doc.RelativeLocation );
                    DocumentManager.ImportLocalDocument( fullPath, doc.Source, doc.Replace, doc.Renaming );
                }
            }

            ResetTmbManager();

            var tmbRootPath = Path.Combine( loadLocation, "Tmb" );
            if( meta.Tmb != null ) {
                foreach(var tmb in meta.Tmb ) {
                    var fullPath = Path.Combine( tmbRootPath, tmb.RelativeLocation );
                    TmbManager.ImportLocalTmb( fullPath, tmb.ReplacePath );
                }
            }

            IsLoading = false;
        }

        private void SaveWorkspace() {
            if( string.IsNullOrEmpty( CurrentWorkspaceLocation ) ) {
                SaveAsWorkspace();
            }
            else {
                Task.Run( async () => {
                    ExportWorkspace();
                } );
            }
        }

        private void SaveAsWorkspace() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".vfxworkspace", "workspace", "vfxworkspace", ( bool ok, string res ) => {
                if( !ok ) return;
                CurrentWorkspaceLocation = res;
                ExportWorkspace();
            } );
        }

        private void ExportWorkspace() {
            var saveLocation = Path.Combine( Path.GetDirectoryName( CurrentWorkspaceLocation ), "VFX_WORKSPACE_TEMP" );
            Directory.CreateDirectory( saveLocation );

            var meta = new WorkspaceMeta {
                Docs = DocumentManager.WorkspaceExport( saveLocation ),
                Tex = TextureManager.WorkspaceExport( saveLocation ),
                Tmb = TmbManager.WorkspaceExport( saveLocation )
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
            oldManager?.Dispose();
        }

        private static void ResetDocumentManager() {
            var oldManager = DocumentManager;
            DocumentManager = new DocumentManager();
            oldManager?.Dispose();
        }

        private static void ResetTmbManager() {
            var oldManager = TmbManager;
            TmbManager = new TmbManager();
            oldManager?.Dispose();
        }
    }
}
