using ImGuiNET;
using System.IO;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class CustomNodeData : UldNodeComponentData {
        private byte[] Data;

        public void Read( BinaryReader reader, int size ) {
            base.Read( reader );
            Data = reader.ReadBytes( size );
        }

        public override void Write( BinaryWriter writer ) {
            base.Write( writer );
            if( Data == null ) return;
            writer.Write( Data );
        }

        public override void Draw( string id ) {
            base.Draw( id );
            ImGui.TextDisabled( $"Custom data of size 0x{Data?.Length:X8}" );
        }
    }
}
