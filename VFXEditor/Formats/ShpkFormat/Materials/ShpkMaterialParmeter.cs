using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Materials {
    public class ShpkMaterialParmeter : IUiItem {
        public readonly ParsedCrc Id = new( "Id" );
        public readonly ParsedShort Offset = new( "Offset" );
        public readonly ParsedShort Size = new( "Size" );

        public int StartSlot => Offset.Value / 4;
        public int EndSlot => ( Offset.Value + Size.Value ) / 4;

        public ShpkMaterialParmeter() { }

        public ShpkMaterialParmeter( BinaryReader reader ) {
            Id.Read( reader );
            Offset.Read( reader );
            Size.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            Offset.Write( writer );
            Size.Write( writer );
        }

        public void Draw() {
            Id.Draw( CommandManager.Shpk, CrcMaps.MaterialParams );
            Offset.Draw( CommandManager.Shpk );
            Size.Draw( CommandManager.Shpk );
        }
    }
}
