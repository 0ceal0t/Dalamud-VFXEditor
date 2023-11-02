using System;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat.Texture {
    [Flags]
    public enum TextureFlags {
        DX11 = 0x8000
    };

    public class MtrlTexture : IUiItem {
        public readonly ParsedString Path = new( "Path" );
        public readonly ParsedFlag<TextureFlags> Flags = new( "Flags", 2 );

        public string Text => string.IsNullOrEmpty( Path.Value ) ? "[NONE]" : Path.Value.Split( "/" )[^1];

        private readonly ushort TempOffset;

        public MtrlTexture() { }

        public MtrlTexture( BinaryReader reader ) {
            TempOffset = reader.ReadUInt16();
            Flags.Read( reader );
        }

        public void ReadString( BinaryReader reader, long stringsStart ) {
            reader.BaseStream.Seek( stringsStart + TempOffset, SeekOrigin.Begin );
            Path.Value = FileUtils.ReadString( reader );
        }

        public void Draw() {
            Path.Draw();
            Flags.Draw();
            Plugin.TextureManager.GetTexture( Path.Value )?.Draw();
        }
    }
}
