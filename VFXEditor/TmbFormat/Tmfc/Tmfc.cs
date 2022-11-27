using ImGuiNET;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.TmbFormat.Entries {
    public class Tmfc : TmbEntry {
        public const string MAGIC = "TMFC";
        public const string DISPLAY_NAME = "TMFC";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => Data.Length;

        private readonly ParsedInt StartOffset = new( "Unknown 1" );
        private readonly ParsedInt UnkCount = new( "Unknown 2" );
        private readonly ParsedInt Unk1 = new( "Unknown 3" );
        private readonly ParsedInt EndOffset = new( "Unknown 4" );
        private readonly ParsedInt Unk2 = new( "Unknown 5" );

        private byte[] Data;
        // int (0)
        // * count
            // float short short int int (0x10)
        // * count
            // float float float int int int (0x18)
        // float float float int int (0x14)

        public Tmfc( bool papEmbedded ) : base( papEmbedded ) { }

        public Tmfc( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            StartOffset.Read( reader );
            UnkCount.Read( reader );
            Unk1.Read( reader );
            EndOffset.Read( reader );
            Unk2.Read( reader );

            var diff = EndOffset.Value - StartOffset.Value;
            // need to add an extra 4 bytes to account for id+time
            reader.ReadAtOffset( StartOffset.Value + 4, ( BinaryReader br ) => {
                Data = br.ReadBytes( diff );
            } );
        }

        protected override List<ParsedBase> GetParsed() => new();

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            var offset = writer.WriteExtra( ( BinaryWriter bw ) => {
                bw.Write( Data );
            }, modifyOffset: 4 );

            UnkCount.Write( writer );
            Unk1.Write( writer );
            EndOffset.Value = offset + Data.Length;
            EndOffset.Write( writer );
            Unk2.Write( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            UnkCount.Draw( id, Command );
            Unk1.Draw( id, Command );
            Unk2.Draw( id, Command );
        }
    }
}
