using Lumina.Misc;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using static VfxEditor.Utils.ShaderUtils;

namespace VfxEditor.Formats.ShpkFormat.Shaders {
    public class ShpkParameterInfo : IUiItem, ITextItem {
        public readonly ShaderFileType Type;
        public uint Id => Crc32.Get( Value.Value, 0xFFFFFFFFu );
        public uint DataSize => ( uint )Size.Value;

        private readonly uint TempId;
        private readonly ParsedString Value = new( "Value" );
        private readonly uint TempStringOffset;
        private readonly ParsedShort Unknown1 = new( "Unknown1" );
        private readonly ParsedShort Unknown2 = new( "Unknown2" );



        public readonly ParsedBool IsTexture = new( "Is Texture", size: 2 );
        public readonly ParsedShort Slot = new( "Slot" );
        public readonly ParsedShort Size = new( "Registers" );

        public ShpkParameterInfo( ShaderFileType type ) {
            Type = type;
        }

        public ShpkParameterInfo( BinaryReader reader, ShaderFileType type ) : this( type ) {
            Dalamud.Log( $"Read location: {reader.BaseStream.Position}" );
            TempId = reader.ReadUInt32(); // Id
            Dalamud.Log( $"thisRead location: {reader.BaseStream.Position}" );
            TempStringOffset = reader.ReadUInt32();
            reader.ReadUInt16(); // string size
            //Unknown1.Read( reader );
            //Unknown2.Read( reader );

            IsTexture.Read( reader );
            Dalamud.Log( $"Subread location: {reader.BaseStream.Position}" );
            Slot.Read( reader );
            Size.Read( reader );
        }

        public void Read( BinaryReader reader, uint parameterOffset ) {
            reader.BaseStream.Position = parameterOffset + TempStringOffset;
            Value.Read( reader );
            if( TempId != Id ) Dalamud.Error( "Ids do not match" );
        }

        public void Write( BinaryWriter writer, List<(long, string)> stringPositions ) {
            writer.Write( Id );
            stringPositions.Add( (writer.BaseStream.Position, Value.Value) );
            writer.Write( 0 ); // placeholder
            writer.Write( ( ushort )Value.Value.Length );

            //Unknown1.Write( writer );
            //Unknown2.Write( writer );

            IsTexture.Write( writer );
            Slot.Write( writer );
            Size.Write( writer );
        }

        public void Draw() {
            Value.Draw();
            IsTexture.Draw();
            //Unknown1.Draw();
            //Unknown2.Draw();
            Slot.Draw();
            Size.Draw();
        }

        public string GetText() => Value.Value;
    }
}
