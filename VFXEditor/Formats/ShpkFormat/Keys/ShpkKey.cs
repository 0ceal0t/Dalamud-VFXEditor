using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Keys {
    public class ShpkKey : IUiItem {
        public readonly ParsedCrc Id = new( "Id" );
        public readonly ParsedCrc DefaultValue = new( "Default Value" );

        public ShpkKey() { }

        public ShpkKey( uint id, uint defaultValue ) {
            Id.Value = id;
            DefaultValue.Value = defaultValue;
        }

        public ShpkKey( BinaryReader reader ) {
            Id.Read( reader );
            DefaultValue.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            DefaultValue.Write( writer );
        }

        public void Draw() {
            Id.Draw( CommandManager.Shpk, CrcMaps.Keys );
            DefaultValue.Draw( CommandManager.Shpk, CrcMaps.Keys );
        }

        public string GetText( int idx ) => CrcMaps.Keys.TryGetValue( Id.Value, out var text ) ? text : $"Key {idx} (0x{Id.Value:X4})";
    }
}
