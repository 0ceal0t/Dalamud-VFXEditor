using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.TextureFormat {
    public partial class TextureManager {
        public IEnumerable<IFileDocument> GetExportDocuments() => PathToTextureReplace.Values;

        public void WorkspaceImport( JObject meta, string loadLocation ) {
            var items = WorkspaceUtils.ReadFromMeta<WorkspaceMetaTex>( meta, "Tex" );
            if( items == null ) return;
            foreach( var item in items ) {
                var fullPath = WorkspaceUtils.ResolveWorkspacePath( item.RelativeLocation, Path.Combine( loadLocation, "Tex" ) );
                ImportAtex( fullPath, item.ReplacePath, item.Height, item.Width, item.Depth, item.MipLevels, item.Format );
            }
        }

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation ) {
            var texRootPath = Path.Combine( saveLocation, "Tex" );
            Directory.CreateDirectory( texRootPath );

            var texId = 0;
            List<WorkspaceMetaTex> texMeta = new();

            foreach( var entry in PathToTextureReplace ) {
                var extension = Path.GetExtension( entry.Value.LocalPath );
                var newPath = $"VFX_{texId++}{extension}";
                var newFullPath = Path.Combine( texRootPath, newPath );
                File.Copy( entry.Value.LocalPath, newFullPath, true );

                texMeta.Add( new WorkspaceMetaTex {
                    Height = entry.Value.Height,
                    Width = entry.Value.Width,
                    Depth = entry.Value.Depth,
                    MipLevels = entry.Value.MipLevels,
                    Format = entry.Value.Format,
                    RelativeLocation = newPath,
                    ReplacePath = entry.Key
                } );
            }

            WorkspaceUtils.WriteToMeta( meta, texMeta.ToArray(), "Tex" );
        }
    }
}
