using ImGuiNET;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat.Table.Dye {
    // TODO
    public class MtrlDyeTableRowExtended {
        public readonly ParsedShort Template = new( "Template" );
        public readonly ParsedFlag<DyeRowFlags> Flags = new( "Flags" );

        public MtrlDyeTableRowExtended() { }

        public MtrlDyeTableRowExtended( BinaryReader reader ) {
            Flags.Read( reader );
            Template.Value = Flags.IntValue >> 5;
        }

        public void Write( BinaryWriter writer ) {
            var value = Flags.IntValue;
            writer.Write( ( ushort )( value & 0x1F | Template.Value << 5 ) );
        }

        public void Draw() {
            ImGui.SetNextItemWidth( 200f );
            if( UiUtils.EnumComboBox( "Template", Plugin.MtrlManager.Templates, Template.Value, out var value ) ) Template.Update( value );
            Flags.Draw();
        }
    }
}
