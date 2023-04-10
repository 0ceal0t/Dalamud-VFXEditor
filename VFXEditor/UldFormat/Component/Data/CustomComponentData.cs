using ImGuiNET;
using System.IO;

namespace VfxEditor.UldFormat.Component.Data {
    public class CustomComponentData : UldComponentData {
        private byte[] Data;

        public void Read( BinaryReader reader, int size ) {
            Data = reader.ReadBytes( size );
        }

        public override void Write( BinaryWriter writer ) {
            if( Data == null ) return;
            writer.Write( Data.Length );
        }

        public override void Draw( string id ) {
            ImGui.TextDisabled( $"Custom data of size 0x{Data?.Length:X8}" );
        }
    }
}
