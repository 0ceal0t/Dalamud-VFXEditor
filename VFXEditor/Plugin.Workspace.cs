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
        public void SaveWorkspace() {
            var saveLocation = @"C:\tmp\test"; // TODO
            Directory.CreateDirectory( saveLocation );

            var meta = new WorkspaceMeta();

            var vfxRootPath = Path.Combine( saveLocation, "VFX" );
            Directory.CreateDirectory( vfxRootPath );

            int docId = 0;
            List<WorkspaceMetaDocument> docMeta = new();
            foreach(var entry in Doc.Docs) {
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
                    Renaming = new()
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

            string metaPath = Path.Combine( saveLocation, "vfx_meta.json" );
            string metaString = JsonConvert.SerializeObject( meta );
            File.WriteAllText( metaPath, metaString );
        }
    }
}
