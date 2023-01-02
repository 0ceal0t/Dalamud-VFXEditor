using ImGuiNET;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;
using System.Collections.Generic;
using System.IO;
using Dalamud.Logging;

namespace VfxEditor.TmbFormat.Entries {
    public class Tmfc : TmbEntry {
        public const string MAGIC = "TMFC";
        public const string DISPLAY_NAME = "TMFC";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => Data.Length;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );

        private int Count = 0;
        private byte[] Data;
        // TODO

        // int (0)
        // * count
            // float short short int int (0x10)
        // * count
            // float float float int int int (0x18)
        // float float float int int (0x14)

        public Tmfc( bool papEmbedded ) : base( papEmbedded ) { }

        public Tmfc( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            var startOffset = reader.ReadInt32();
            Count = reader.ReadInt32();
            Unk1.Read( reader );
            var endOffset = reader.ReadInt32();
            Unk2.Read( reader );

            var diff = endOffset - startOffset;
            // need to add an extra 4 bytes to account for id+time
            reader.ReadAtOffset( startOffset + 4, ( BinaryReader br ) => {
                Data = br.ReadBytes( diff );
                PluginLog.Log( $"{Data.Length}" );
            } );
        }

        protected override List<ParsedBase> GetParsed() => new();

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            var offset = writer.WriteExtra( ( BinaryWriter bw ) => {
                bw.Write( Data );
            }, modifyOffset: 4 );

            writer.Write( Count );
            Unk1.Write( writer );
            writer.Write( offset + Data.Length );
            Unk2.Write( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            Unk1.Draw( id, Command );
            Unk2.Draw( id, Command );
        }
    }
}
