using Dalamud.Interface.Utility.Raii;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat.Sound.Data {
    public class SoundBusDucking {
        private byte Size = 0x10;
        public readonly ParsedByte Number = new( "Number" );
        private readonly ParsedByte Unknown1 = new( "Unknown 1" );
        private readonly ParsedByte Unknown2 = new( "Unknown 2" );
        public readonly ParsedInt FadeTime = new( "Fade Time" );
        public readonly ParsedFloat Volume = new( "Volume" );
        private readonly ParsedSByte Unknown3 = new( "Unknown 3" );
        private readonly ParsedSByte Unknown4 = new( "Unknown 4" );
        private readonly ParsedSByte Unknown5 = new( "Unknown 5" );
        private readonly ParsedSByte Unknown6 = new( "Unknown 6" );

        public void Read( BinaryReader reader ) {
            Size = reader.ReadByte();
            Number.Read( reader );
            Unknown1.Read( reader );
            Unknown2.Read( reader );
            FadeTime.Read( reader );
            Volume.Read( reader );
            Unknown3.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
            Unknown6.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Size );
            Number.Write( writer );
            Unknown1.Write( writer );
            Unknown2.Write( writer );
            FadeTime.Write( writer );
            Volume.Write( writer );
            Unknown3.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
            Unknown6.Write( writer );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "BusDucking" );

            Number.Draw();
            Unknown1.Draw();
            Unknown2.Draw();
            FadeTime.Draw();
            Volume.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
            Unknown6.Draw();
        }
    }
}
