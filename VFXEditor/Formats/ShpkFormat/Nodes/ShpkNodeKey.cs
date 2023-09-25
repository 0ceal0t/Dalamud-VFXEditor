using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Nodes {
    public class ShpkNodeKey : IUiItem {
        public readonly ParsedCrc Id = new( "##Id" );

        public ShpkNodeKey() { }

        public ShpkNodeKey( BinaryReader reader ) {
            Id.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
        }

        public void Draw() {
            Id.Draw( CommandManager.Shpk, CrcMaps.Keys );
        }
    }
}
