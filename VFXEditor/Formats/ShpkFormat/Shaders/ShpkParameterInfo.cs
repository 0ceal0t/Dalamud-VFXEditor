using Dalamud.Logging;
using Lumina.Misc;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Shaders {
    public class ShpkParameterInfo : IUiItem, ITextItem {
        public uint Id => Crc32.Get( Value.Value, 0xFFFFFFFFu );

        private readonly uint TempId;
        private readonly ParsedString Value = new( "Value" );
        private readonly int TempStringOffset;

        private readonly ParsedShort Slot = new( "Slot" );
        private readonly ParsedShort Size = new( "Size" );

        public ShpkParameterInfo() { }

        public ShpkParameterInfo( BinaryReader reader ) : this() {
            TempId = reader.ReadUInt32(); // Id
            TempStringOffset = reader.ReadInt32();
            reader.ReadInt32(); // string size
            Slot.Read( reader );
            Size.Read( reader );
        }

        public void Read( BinaryReader reader, uint parameterOffset ) {
            reader.BaseStream.Seek( parameterOffset + TempStringOffset, SeekOrigin.Begin );
            Value.Read( reader );

            if( TempId != Id ) PluginLog.Error( "Ids do not match" );
        }

        public void Write( BinaryWriter writer ) {

        }

        public void Draw() {
            Value.Draw( CommandManager.Shpk );
            Slot.Draw( CommandManager.Shpk );
            Size.Draw( CommandManager.Shpk );
        }

        public string GetText() => Value.Value;
    }
}
