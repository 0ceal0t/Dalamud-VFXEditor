using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Materials {
    public class ShpkMaterialParmeter : IUiItem {
        public readonly ParsedId Id = new( "Id" );
        private readonly ParsedShort Offset = new( "Offset" );
        private readonly ParsedShort Size = new( "Size" );

        public ShpkMaterialParmeter() { }

        public ShpkMaterialParmeter( BinaryReader reader ) {
            Id.Read( reader );
            Offset.Read( reader );
            Size.Read( reader );
        }

        public void Write( BinaryWriter writer ) {

        }

        public void Draw() {
            Id.Draw( CommandManager.Shpk, CrcMaps.MaterialParams );
            Offset.Draw( CommandManager.Shpk );
            Size.Draw( CommandManager.Shpk );
        }
    }
}
