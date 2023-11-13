using ImGuiNET;
using System;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat.Table {
    [Flags]
    public enum DyeRowFlags {
        Apply_Diffuse = 0x01,
        Apply_Specular = 0x02,
        Apply_Emissive = 0x04,
        Apply_Gloss = 0x08,
        Apply_Specular_Strength = 0x10
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
            ImGui.SetNextItemWidth( 200f );
            if( UiUtils.EnumComboBox( "Template", Plugin.MtrlManager.Templates, Template.Value, out var value ) ) {
                Template.Update( value );
            }

            Flags.Draw();
        }
    }
}
