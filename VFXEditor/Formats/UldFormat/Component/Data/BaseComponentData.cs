using ImGuiNET;
using System.IO;

namespace VfxEditor.UldFormat.Component.Data {
    public class BaseComponentData : UldGenericData {
        private byte[] Data;

        public void Read( BinaryReader reader, int size ) {
            Data = reader.ReadBytes( size );
        }

        public override void Write( BinaryWriter writer ) {
            if( Data == null ) return;
            writer.Write( Data );
        }

        public override void Draw() {
            ImGui.TextDisabled( $"Data of size 0x{Data?.Length:X8}" );
        }
    }
}
