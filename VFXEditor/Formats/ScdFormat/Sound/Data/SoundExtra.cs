using Dalamud.Interface.Utility.Raii;
using System;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat.Sound.Data {
    [Flags]
    public enum SoundExtraFilter {
        Unknown_2 = 0x01,
        DualShock_A = 0x02,
        Controller_Sound_Only = 0x04, //diverts if connected
        DualShock_B = 0x08,
        Unknown_3 = 0x10,
        Use_High_Shelf_Filter = 0x20,
        Unknown_4 = 0x40,
        Unknown_5 = 0x80 //RandomWind
    }
    public class SoundExtra {
        public readonly ParsedByte Version = new( "Version" );
        private readonly ParsedByte Unknown1 = new( "Unknown 1" ); //Reserve 1
        private ushort Size = 0x10;
        public readonly ParsedInt PlayTimeLength = new( "Play Time Length" );
        private readonly ParsedFlag<SoundExtraFilter> SoundFilter = new( "Sound Filter" );
        private readonly ParsedFloat Unknown6 = new( "Unknown 6" ); //0.0 or 999.0

        public void Read( BinaryReader reader ) {
            Version.Read( reader );
            Unknown1.Read( reader );
            Size = reader.ReadUInt16();
            PlayTimeLength.Read( reader );
            SoundFilter.Read( reader );
            Unknown6.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Version.Write( writer );
            Unknown1.Write( writer );
            writer.Write( Size );
            PlayTimeLength.Write( writer );
            SoundFilter.Write( writer );
            Unknown6.Write( writer );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Extra" );

            Version.Draw();
            Unknown1.Draw();
            PlayTimeLength.Draw();
            SoundFilter.Draw();
            Unknown6.Draw();
        }
    }
}
