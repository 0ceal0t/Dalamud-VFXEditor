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

    public class MtrlDyeTableRow {
        public readonly MtrlTables Tables;
        public readonly ParsedShort Template = new( "Template" );
        public readonly ParsedFlag<DyeRowFlags> Flags = new( "Flags", 2 );

        public MtrlDyeTableRow( MtrlTables tables ) {
            Tables = tables;
        }

        public void Read( BinaryReader reader ) {
            Flags.Read( reader );
            Template.Value = Flags.IntValue >> 5;

            if( Tables.Extended ) reader.ReadUInt16(); // temp
        }

        public void Write( BinaryWriter writer ) {
            var value = Flags.IntValue;
            writer.Write( ( ushort )( value & 0x1F | Template.Value << 5 ) );

            if( Tables.Extended ) FileUtils.Pad( writer, 2 ); // temp
        }

        public void Draw() {
            ImGui.SetNextItemWidth( 200f );
            if( UiUtils.EnumComboBox( "Template", Plugin.MtrlManager.Templates, Template.Value, out var value ) ) Template.Update( value );
            Flags.Draw();
        }
    }
}
