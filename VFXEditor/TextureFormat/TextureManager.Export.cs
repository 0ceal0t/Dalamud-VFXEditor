using System.Collections.Generic;
using System.IO;

using VfxEditor.Utils;
using VfxEditor.TexTools;

namespace VfxEditor.TextureFormat {
    public partial class TextureManager {
        public void PenumbraExport( string modFolder, bool exportTex ) {
            if( !exportTex ) return;
            foreach( var entry in PathToTextureReplace ) {
                var localPath = entry.Value.LocalPath;
                var path = entry.Key;
                if( string.IsNullOrEmpty( localPath ) || string.IsNullOrEmpty( path ) ) continue;

                PenumbraUtils.CopyFile( localPath, modFolder, path );
            }
        }

        public void TextoolsExport( BinaryWriter writer, bool exportTex, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            if( !exportTex ) return;
            foreach( var entry in PathToTextureReplace ) {
                var localPath = entry.Value.LocalPath;
                var path = entry.Key;
                if( string.IsNullOrEmpty( localPath ) || string.IsNullOrEmpty( path ) ) continue;

                using var file = File.Open( localPath, FileMode.Open );
                using var texReader = new BinaryReader( file );
                using var texMs = new MemoryStream();
                using var texWriter = new BinaryWriter( texMs );
                texWriter.Write( TexToolsUtils.CreateType2Data( texReader.ReadBytes( ( int )file.Length ) ) );
                var modData = texMs.ToArray();
                simpleParts.Add( TexToolsUtils.CreateModResource( path, modOffset, modData.Length ) );
                writer.Write( modData );
                modOffset += modData.Length;
            }
        }

        public WorkspaceMetaTex[] WorkspaceExport( string saveLocation ) {
            var texRootPath = Path.Combine( saveLocation, "Tex" );
            Directory.CreateDirectory( texRootPath );

            var texId = 0;
            List<WorkspaceMetaTex> texMeta = new();

            foreach( var entry in PathToTextureReplace ) {
                var newPath = $"VFX_{texId++}.atex";
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

            return texMeta.ToArray();
        }
    }
}
