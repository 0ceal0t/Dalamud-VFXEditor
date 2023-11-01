using System;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Table {
    [Flags]
    public enum DyeRowFlags {
        Diffuse = 0x01,
        Specular = 0x02,
        Emissive = 0x04,
        Gloss = 0x08,
        SpecularStrength = 0x10
    }

    public class MtrlDyeTableRow : IUiItem {
        public const int Size = 2;

        public readonly ParsedShort Template = new( "Template" );
        public readonly ParsedFlag<DyeRowFlags> Flags = new( "Flags", 2 );

        public MtrlDyeTableRow() { }

        public MtrlDyeTableRow( BinaryReader reader ) {
            Flags.Read( reader );
            Template.Value = Flags.IntValue >> 5;
        }

        public void Write( BinaryWriter writer ) {
            var value = Flags.IntValue;
            writer.Write( ( ushort )( ( value & 0x1F ) | ( Template.Value << 5 ) ) );
        }

        public void Draw() {
            Template.Draw();
            Flags.Draw();
        }
    }
}
