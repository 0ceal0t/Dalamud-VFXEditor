using ImGuiNET;
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

        public MtrlDyeTableRow( MtrlTables tables ) {
            Tables = tables;
        }

        public void Read( BinaryReader reader ) {
            if( !Tables.Extended ) {
                var value = reader.ReadUInt16();
                Flags.Value = ( DyeRowFlags )( object )( value & 0b11111 );
                Template.Value = value >> 5;
            }
            else {
                var value = reader.ReadUInt32();
                Flags.Value = ( DyeRowFlags )( object )( int )( value & 0b11111 );
                Template.Value = ( int )( value >> 16 );
            }

            // TODO: still some work

            // 322B // old (16)
            // 11001000101011
            // 00110010001 | 01011

            // 0x191000B // new (32)
            // 1100100010000000000001011
            // 110010001 | 00000000000 | 01011
        }

        public void Write( BinaryWriter writer ) {
            if( !Tables.Extended ) {
                writer.Write( ( ushort )( ( Flags.IntValue & 0b11111 ) | ( Template.Value << 5 ) ) );
            }
            else {
                writer.Write( ( uint )( ( Flags.IntValue & 0b11111 ) | ( Template.Value << 16 ) ) );
            }
        }

        public void Draw() {
            ImGui.SetNextItemWidth( 200f );
            if( UiUtils.EnumComboBox( "Template", Plugin.MtrlManager.Templates, Template.Value, out var value ) ) Template.Update( value );
            Flags.Draw();
        }
    }
}
