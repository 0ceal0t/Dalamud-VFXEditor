using Dalamud.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

                var oldTex = TexManager;
                TexManager = new TextureManager( this );
                oldTex.Dispose();

                var oldDoc = DocManager;
                DocManager = new PluginDocumentManager( this );
                oldDoc.Dispose();

                IsLoading = false;
            } );
        }

        public void OpenWorkspace() {
            ImportFileDialog( "JSON File (*.json)|*.json*|All files (*.*)|*.*", "Select workspace file", ( string path ) =>
            {
                try {
                    var loadLocation = CurrentWorkspaceLocation = Path.GetDirectoryName( path );
                    // get meta

                    // create new texmanager
                    // copy over .atex and add them to maps

                    // create new docmanager
                    // copy over .avfx and add them to maps (if applicable)
                    // set source and replace
                    // read .avfx and create new UIMain
                    // remove default doc if > 0 doc added

                    //var meta = JsonConvert.DeserializeObject<WorkspaceMeta>();
                }
                catch(Exception e) {
                    PluginLog.LogError( "Could not load workspace", e );
                }
            } );
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
            SaveFolderDialog( "JSON File (*.json)|*.json*|All files (*.*)|*.*", "Select save location", ( string path ) =>
            {
                CurrentWorkspaceLocation = Path.GetDirectoryName( path );
                ExportWorkspace();
            } );
        }

        public void ExportWorkspace() {
            var saveLocation = CurrentWorkspaceLocation;
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
            foreach(var entry in TexManager.GamePathReplace) {
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
        }
    }
}
