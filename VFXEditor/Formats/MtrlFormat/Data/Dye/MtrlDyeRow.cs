using System;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.MtrlFormat.Data.Dye {
    [Flags]
    public enum DyeRowFlags {
        Diffuse = 0x01,
        Specular = 0x02,
        Emissive = 0x04,
        Unknown = 0x08,
        Metalness = 0x10,
        Rougness = 0x20,
        Sheen_Rate = 0x40,
        Sheen_Tint_Rate = 0x80,
        Sheen_Aperature = 0x100,
        Anisotropy = 0x200,
        Sphere_Map_Index = 0x400,
        Sphere_Map_Mask = 0x800
    }

    public class MtrlDyeRow {
        public readonly ParsedShort Template = new( "Template" );
        public readonly ParsedFlag<DyeRowFlags> Flags = new( "Flags" );
        public readonly ParsedInt Channel = new( "Channel" );

        public void Read( BinaryReader reader ) {
            var value = reader.ReadUInt32();
            Flags.Value = ( DyeRowFlags )( object )( int )( value & 0b111111111111 ); // 12 bits
            Template.Value = ( int )( ( value >> 16 ) & 0b11111111111 ); // 11 bits
            Channel.Value = ( int )( ( value >> 27 ) & 0b11 );
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( ( uint )( Flags.IntValue | ( Template.Value << 16 ) | ( Channel.Value << 27 ) ) );
        }

        public void Draw() {
            // TODO: template
            Channel.Draw();
            Flags.Draw();
        }
    }
}
