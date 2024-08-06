using System;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat.Data.Dye {
    [Flags]
    public enum DyeRowFlags_Legacy {
        Diffuse = 0x01,
        Specular = 0x02,
        Emissive = 0x04,
        Gloss = 0x08,
        Specular_Strength = 0x10
    }

    public class MtrlDyeRowLegacy {
        public readonly ParsedShort Template = new( "Template" );
        public readonly ParsedFlag<DyeRowFlags_Legacy> Flags = new( "Flags" );

        public void Read( BinaryReader reader ) {
            var value = reader.ReadUInt16();
            Flags.Value = ( DyeRowFlags_Legacy )( object )( value & 0b11111 );
            Template.Value = value >> 5;
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( ( ushort )( Flags.IntValue | ( Template.Value << 5 ) ) );
        }

        public void Draw() {
            if( UiUtils.EnumComboBox( "Template", Plugin.MtrlManager.Templates, Template.Value, out var value ) ) Template.Update( value );
            Flags.Draw();
        }
    }
}
