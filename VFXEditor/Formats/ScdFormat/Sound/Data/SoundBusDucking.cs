using Dalamud.Interface.Utility.Raii;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat.Sound.Data {
    public class SoundBusDucking {
        private byte Size = 0x10;
        public readonly ParsedByte Number = new( "Number" );
        private readonly ParsedReserve Reserve1 = new( 2 );
        public readonly ParsedInt FadeTime = new( "Fade Time" );
        public readonly ParsedFloat Volume = new( "Volume" );
        private uint Reserve2;

        public void Read( BinaryReader reader ) {
            Size = reader.ReadByte();
            Number.Read( reader );
            Reserve1.Read( reader );
            FadeTime.Read( reader );
            Volume.Read( reader );
            Reserve2 = reader.ReadUInt32();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Size );
            Number.Write( writer );
            Reserve1.Write( writer );
            FadeTime.Write( writer );
            Volume.Write( writer );
            writer.Write( Reserve2 );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "BusDucking" );

            Number.Draw();
            FadeTime.Draw();
            Volume.Draw();
        }
    }
}
