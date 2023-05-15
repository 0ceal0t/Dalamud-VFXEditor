using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Ui.Components;

namespace VfxEditor.TmbFormat.Entries {
    public class Tmfc : TmbEntry {
        public const string MAGIC = "TMFC";
        public const string DISPLAY_NAME = "TMFC";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => Data.Count == 0 ? 0 : Data.Select( x => x.Size ).Sum();

        private readonly ParsedInt Unk1 = new( "Unknown 2" );
        private readonly ParsedInt Unk2 = new( "Unknown 3" );

        public readonly List<TmfcData> Data = new();
        private readonly SimpleSplitView<TmfcData> DataSplitView;

        public Tmfc( bool papEmbedded ) : base( papEmbedded ) { }

        public Tmfc( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            var startOffset = reader.ReadInt32();
            var dataCount = reader.ReadInt32();
            Unk1.Read( reader );
            var endOffset = reader.ReadInt32();
            Unk2.Read( reader );

            var diff = endOffset - startOffset;
            // need to add an extra 4 bytes to account for id+time
            reader.ReadAtOffset( startOffset + 4, ( BinaryReader br ) => {
                for( var i = 0; i < dataCount; i++ ) {
                    Data.Add( new TmfcData( br, papEmbedded ) );
                }

                foreach( var data in Data ) data.ReadRows( br );
            } );

            DataSplitView = new( "Entry", Data, false, false );
        }

        protected override List<ParsedBase> GetParsed() => new();

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            var offset = writer.WriteExtra( ( BinaryWriter bw ) => {
                foreach( var data in Data ) data.Write( bw );

                foreach( var data in Data ) data.WriteRows( bw );
            }, modifyOffset: 4 );

            writer.Write( Data.Count );
            Unk1.Write( writer );
            writer.Write( offset + ExtraSize );
            Unk2.Write( writer );
        }

        public override void Draw() {
            DrawHeader();
            Unk1.Draw( Command );
            Unk2.Draw( Command );

            using var child = ImRaii.Child( "Child", new Vector2( -1, -1 ), true );
            using var _ = ImRaii.PushId( "Data" );
            DataSplitView.Draw();
        }
    }
}
