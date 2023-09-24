using Dalamud.Logging;
using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Nodes {
    public class ShpkAlias : IUiItem {
        private uint Selector;
        private uint NodeIdx;

        public ShpkAlias() { }

        public ShpkAlias( BinaryReader reader ) {
            Selector = reader.ReadUInt32();
            NodeIdx = reader.ReadUInt32();

            PluginLog.Log( $"Alias > {Selector:X8} {NodeIdx:X8}" );
        }

        public void Write( BinaryWriter writer ) {

        }

        public void Draw() {

        }
    }
}
