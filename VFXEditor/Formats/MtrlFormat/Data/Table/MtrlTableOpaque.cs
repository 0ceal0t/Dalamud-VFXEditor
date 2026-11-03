using Dalamud.Bindings.ImGui;
using System.IO;

namespace VfxEditor.Formats.MtrlFormat.Data.Table {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/e93093f/Files/Materials/OpaqueColorTable.cs

    public class MtrlTableOpaque : MtrlTableBase {
        private readonly int DimensionLogs;
        private readonly byte[] Data;

        public MtrlTableOpaque( MtrlFile file, BinaryReader reader, long dataEnd, int dimensionLogs ) : base( file ) {
            DimensionLogs = dimensionLogs;
            Data = reader.ReadBytes( ( int )( dataEnd - reader.BaseStream.Position ) );
        }

        public override bool IsValidDimensionLogs( int logs ) => logs == DimensionLogs;

        public override int DefaultDimensionLogs => DimensionLogs;

        public override void Write( BinaryWriter writer ) {
            if( File.ColorTableEnabled ) writer.Write( Data );
        }

        public override void Draw() {
            ImGui.TextDisabled( $"Unsupported color table dimensions (0x{DimensionLogs:X2}, {Data.Length} bytes), it will be saved unchanged" );
        }
    }
}