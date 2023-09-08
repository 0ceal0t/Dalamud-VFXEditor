using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.TextureFormat.Textures {
    public class TextureReplace : IFileDocument {
        public readonly string LocalPath;
        public readonly string ReplacePath;
        public readonly int Height;
        public readonly int Width;
        public readonly int Depth;
        public readonly int MipLevels;
        public readonly TextureFormat Format;

        public TextureReplace( string localPath, string replacePath, int height, int width, int depth, int mips, TextureFormat format ) {
            LocalPath = localPath;
            ReplacePath = replacePath;
            Height = height;
            Width = width;
            Depth = depth;
            MipLevels = mips;
            Format = format;
        }

        public string GetExportSource() => "";

        public string GetExportReplace() => ReplacePath;

        public void PenumbraExport( string modFolder, Dictionary<string, string> files ) {
            if( string.IsNullOrEmpty( LocalPath ) || string.IsNullOrEmpty( ReplacePath ) ) return;

            PenumbraUtils.CopyFile( LocalPath, modFolder, ReplacePath, files );
        }

        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            if( string.IsNullOrEmpty( LocalPath ) || string.IsNullOrEmpty( ReplacePath ) ) return;

            using var file = File.Open( LocalPath, FileMode.Open );
            using var texReader = new BinaryReader( file );
            using var texMs = new MemoryStream();
            using var texWriter = new BinaryWriter( texMs );
            texWriter.Write( TexToolsUtils.CreateType2Data( texReader.ReadBytes( ( int )file.Length ) ) );
            var modData = texMs.ToArray();
            simpleParts.Add( TexToolsUtils.CreateModResource( ReplacePath, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }

        public bool CanExport() => true;
    }
}
