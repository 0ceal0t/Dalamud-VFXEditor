using Dalamud.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using VFXEditor.Data.Texture;
using VFXSelect.UI;

namespace VFXEditor {
    public struct WorkspaceMeta {
        public WorkspaceMetaTex[] Tex;
        public WorkspaceMetaDocument[] Docs;
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

    public partial class Plugin {
        public string CurrentWorkspaceLocation = "";

        public void NewWorkspace() {
            Task.Run( async () => {
                IsLoading = true;
                CurrentWorkspaceLocation = "";

                var oldTex = TexManager;
                TexManager = new TextureManager( this );
                oldTex.Dispose();

                var oldDoc = DocManager;
                DocManager = new DocumentManager( this );
                oldDoc.Dispose();

                IsLoading = false;
            } );
        }

        public void OpenWorkspace() {
            DialogManager.OpenFileDialog( "Select a Workspace FIle", "Workspace{.vfxworkspace,.json}", ( bool ok, string res ) =>
            {
                if( !ok ) return;
                try {
                    var selectedFile = new FileInfo( res );
                    var dir = Path.GetDirectoryName( res );

                    if(selectedFile.Extension == ".json") // OLD + DEPRECATED
                    {
                        OpenWorkspaceFolder( dir );
                        CurrentWorkspaceLocation = dir.TrimEnd( Path.DirectorySeparatorChar ) + ".vfxworkspace"; // move to new format
                    }
                    else if(selectedFile.Extension == ".vfxworkspace") // NEW
                    {
                        var tempDir = Path.Combine( dir, "VFX_WORKSPACE_TEMP" );
                        ZipFile.ExtractToDirectory( res, tempDir, true );
                        OpenWorkspaceFolder( tempDir );
                        Directory.Delete( tempDir, true );
                        CurrentWorkspaceLocation = res;


                    }
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Could not load workspace", e );
                }
            } );
        }

        private void OpenWorkspaceFolder(string loadLocation) {
            string metaPath = Path.Combine( loadLocation, "vfx_workspace.json" );
            if( !File.Exists( metaPath ) )
            {
                PluginLog.Log( "vfx_workspace.json does not exist" );
                return;
            }
            var meta = JsonConvert.DeserializeObject<WorkspaceMeta>( File.ReadAllText( metaPath ) );

            IsLoading = true;

            var oldTex = TexManager;
            TexManager = new TextureManager( this );
            oldTex.Dispose();

            var texRootPath = Path.Combine( loadLocation, "Tex" );
            foreach( var tex in meta.Tex )
            {
                var fullPath = Path.Combine( texRootPath, tex.RelativeLocation );
                TexManager.ImportReplaceTexture( fullPath, tex.ReplacePath, tex.Height, tex.Width, tex.Depth, tex.MipLevels, tex.Format );
            }

            var oldDoc = DocManager;
            DocManager = new DocumentManager( this );
            oldDoc.Dispose();

            var defaultDoc = DocManager.ActiveDoc;

            var vfxRootPath = Path.Combine( loadLocation, "VFX" );
            foreach( var doc in meta.Docs )
            {
                var fullPath = ( doc.RelativeLocation == "" ) ? "" : Path.Combine( vfxRootPath, doc.RelativeLocation );
                DocManager.ImportLocalDoc( fullPath, doc.Source, doc.Replace, doc.Renaming );
            }

            if( DocManager.Docs.Count > 1 )
            {
                DocManager.RemoveDoc( defaultDoc );
            }

            IsLoading = false;
        }

        public void SaveWorkspace() {
            if(string.IsNullOrEmpty(CurrentWorkspaceLocation)) {
                SaveAsWorkspace();
            }
            else {
                Task.Run( async () =>  {
                    ExportWorkspace();
                } );
            }
        }

        public void SaveAsWorkspace() {
             DialogManager.SaveFileDialog( "Select a Save Location", ".vfxworkspace", "workspace", "vfxworkspace", ( bool ok, string res ) =>
             {
                if( !ok ) return;
                CurrentWorkspaceLocation = res;
                ExportWorkspace();
             } );
        }

        public void ExportWorkspace() {
            var saveLocation = Path.Combine( Path.GetDirectoryName( CurrentWorkspaceLocation ), "VFX_WORKSPACE_TEMP" );
            Directory.CreateDirectory( saveLocation );

            var meta = new WorkspaceMeta();

            var vfxRootPath = Path.Combine( saveLocation, "VFX" );
            Directory.CreateDirectory( vfxRootPath );

            int docId = 0;
            List<WorkspaceMetaDocument> docMeta = new();
            foreach(var entry in DocManager.Docs) {
                string newPath = "";
                if( entry.Main != null ) {
                    newPath = $"VFX_{docId++}.avfx";
                    var newFullPath = Path.Combine( vfxRootPath, newPath );
                    File.WriteAllBytes( newFullPath, entry.Main.AVFX.ToAVFX().ToBytes() );
                }
                docMeta.Add( new WorkspaceMetaDocument
                {
                    Source = entry.Source,
                    Replace = entry.Replace,
                    RelativeLocation = newPath,
                    Renaming = (entry.Main == null) ? new Dictionary<string, string>() : entry.Main.GetRenamingMap()
                } );
            }
            meta.Docs = docMeta.ToArray();

            var texRootPath = Path.Combine( saveLocation, "Tex" );
            Directory.CreateDirectory( texRootPath );

            int texId = 0;
            List<WorkspaceMetaTex> texMeta = new();
            foreach(var entry in TexManager.PathToTextureReplace ) {
                var newPath = $"VFX_{texId++}.atex";
                var newFullPath = Path.Combine( texRootPath, newPath );
                File.Copy( entry.Value.localPath, newFullPath, true );
                texMeta.Add( new WorkspaceMetaTex
                {
                    Height = entry.Value.Height,
                    Width = entry.Value.Width,
                    Depth = entry.Value.Depth,
                    MipLevels = entry.Value.MipLevels,
                    Format = entry.Value.Format,
                    RelativeLocation = newPath,
                    ReplacePath = entry.Key
                } );
            }
            meta.Tex = texMeta.ToArray();

            string metaPath = Path.Combine( saveLocation, "vfx_workspace.json" );
            string metaString = JsonConvert.SerializeObject( meta );
            File.WriteAllText( metaPath, metaString );

            if( File.Exists( CurrentWorkspaceLocation ) ) File.Delete( CurrentWorkspaceLocation );
            ZipFile.CreateFromDirectory( saveLocation, CurrentWorkspaceLocation );
            Directory.Delete( saveLocation, true );
        }
    }
}
