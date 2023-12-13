using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat.Texture {
    [Flags]
    public enum TextureFlags {
        DX11 = 0x8000
    };

    public class MtrlTexture : IUiItem {
        public readonly ParsedString Path = new( "Path" );
        public readonly ParsedFlag<TextureFlags> Options = new( "Options", 2 );
        public readonly ParsedUIntHex Flags = new( "Flags" );

        public string Text => string.IsNullOrEmpty( Path.Value ) ? "[NONE]" : Path.Value.Split( "/" )[^1];

        private readonly ushort TempOffset;

        public MtrlTexture() { }

        public MtrlTexture( BinaryReader reader ) {
            TempOffset = reader.ReadUInt16();
            var flags = ( uint )reader.ReadUInt16();

            Flags.Value = Masked( flags );
            Options.Value = ( TextureFlags )flags;
        }

        public void ReadString( BinaryReader reader, long stringsStart ) {
            reader.BaseStream.Position = stringsStart + TempOffset;
            Path.Value = FileUtils.ReadString( reader );
        }

        public void Write( BinaryWriter writer, Dictionary<long, string> stringPositions ) {
            stringPositions[writer.BaseStream.Position] = Path.Value;
            writer.Write( ( ushort )0 ); // placeholder

            var flags = Masked( Flags.Value );
            flags |= ( uint )Options.Value;
            writer.Write( ( ushort )flags );
        }

        public void Draw() {
            Path.Draw();
            Options.Draw();
            Flags.Draw();
            Plugin.TextureManager.GetTexture( Path.Value )?.Draw();
        }

        private static uint Masked( uint flags ) => flags & ( ~0x8000u );
    }
}
