using ImGuiNET;
using OtterGui.Raii;
using System.IO;

namespace VfxEditor.PhybFormat.Simulator {
    public class PhybSimulator {
        public readonly PhybFile File;

        public readonly PhybSimulatorParams Params;

        public PhybSimulator( PhybFile file, BinaryReader reader ) {
            File = file;

            var startPos = reader.BaseStream.Position;

            var numCollision = reader.ReadByte();
            var numCollisionConnector = reader.ReadByte();
            var numChain = reader.ReadByte();
            var numConnector = reader.ReadByte();
            var numAttract = reader.ReadByte();
            var numPin = reader.ReadByte();
            var numSpring = reader.ReadByte();
            var numPostAlignment = reader.ReadByte();

            Params = new( file, reader );

            var collisionOffset = reader.ReadUInt32();
            var collisionConnectorOffset = reader.ReadUInt32();
            var chainOffset = reader.ReadUInt32();
            var connectorOffset = reader.ReadUInt32();
            var attractOffset = reader.ReadUInt32();
            var pinOffset = reader.ReadUInt32();
            var springOffset = reader.ReadUInt32();
            var postAlignmentOffset = reader.ReadUInt32();
        }

        public void Write( BinaryWriter writer ) {

        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Simulator" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) Params.Draw();
            }
        }
    }
}
