using System;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat.Table.Dye {
    [Flags]
    public enum DyeRowFlags {
        Apply_Diffuse = 0x01,
        Apply_Specular = 0x02,
        Apply_Emissive = 0x04,
        Apply_Gloss = 0x08,
        Apply_Specular_Strength = 0x10
    }

    public partial class MtrlDyeTableRow {
        public readonly MtrlTables Tables;
        public readonly ParsedShort Template = new( "Template" );
        public readonly ParsedFlag<DyeRowFlags> Flags = new( "Flags" );
        public readonly ParsedInt Channel = new( "Channel" );

        public MtrlDyeTableRow( MtrlTables tables ) {
            Tables = tables;
        }

        public void Read( BinaryReader reader ) {
            if( Tables.Legacy ) {
                var value = reader.ReadUInt16();
                Flags.Value = ( DyeRowFlags )( object )( value & 0b11111 );
                Template.Value = value >> 5;
            }
            else { // DT
                var value = reader.ReadUInt32();
                Flags.Value = ( DyeRowFlags )( object )( int )( value & 0b111111111111 ); // 12 bits
                Template.Value = ( int )( ( value >> 16 ) & 0b11111111111 ); // 11 bits
                Channel.Value = ( int )( ( value >> 27 ) & 0b11 );

                /*
                12 bits flags
                4 bits unk
                11 bits template id
                2 bits dye channel selector
                3 bits unknown
                */
            }
        }

        public void Write( BinaryWriter writer ) {
            if( Tables.Legacy ) {
                writer.Write( ( ushort )( Flags.IntValue | ( Template.Value << 5 ) ) );
            }
            else { // DT
                writer.Write( ( uint )( Flags.IntValue | ( Template.Value << 16 ) | ( Channel.Value << 27 ) ) );
            }
        }

        public void Draw() {
            if( UiUtils.EnumComboBox( "Template", Plugin.MtrlManager.Templates, Template.Value, out var value ) ) Template.Update( value );
            if( !Tables.Legacy ) Channel.Draw();
            Flags.Draw();
        }
    }
}
