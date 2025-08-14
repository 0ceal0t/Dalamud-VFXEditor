using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.TmbFormat.Tmfcs {
    public class Tmfc : TmbEntry {
        public const string MAGIC = "TMFC";
        public const string DISPLAY_NAME = "TMFC";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => Data.Count == 0 ? 0 : Data.Select( x => x.Size ).Sum();

        private readonly ParsedInt Unk1 = new( "Unknown 2" );
        private readonly ParsedInt Unk2 = new( "Unknown 3" );

        public readonly List<TmfcData> Data = [];
        private readonly UiSplitView<TmfcData> DataSplitView;

        public Tmfc( TmbFile file ) : base( file ) { }

        public Tmfc( TmbFile file, TmbReader reader ) : base( file, reader ) {
            var startOffset = reader.ReadInt32();
            var dataCount = reader.ReadInt32();
            Unk1.Read( reader );
            var endOffset = reader.ReadInt32();
            Unk2.Read( reader );

            var diff = endOffset - startOffset;
            // need to add an extra 4 bytes to account for id+time
            reader.ReadAtOffset( startOffset + 4, ( BinaryReader br ) => {
                for( var i = 0; i < dataCount; i++ ) {
                    Data.Add( new TmfcData( br, File ) );
                }

                foreach( var data in Data ) data.ReadRows( br );
            } );

            DataSplitView = new( "Entry", Data, false );
        }

        protected override List<ParsedBase> GetParsed() => [];

        public override void Write( TmbWriter writer ) {
            base.Write( writer );

            var offset = writer.WriteExtra( ( BinaryWriter bw ) => {
                foreach( var data in Data ) data.Write( bw );

                foreach( var data in Data ) data.WriteRows( bw );
            }, modifyOffset: 4 );

            writer.Write( Data.Count );
            Unk1.Write( writer );
            writer.Write( offset + ExtraSize );
            Unk2.Write( writer );
        }

        public override void DrawBody() {
            base.DrawBody();

            Unk1.Draw();
            Unk2.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            using var _ = ImRaii.PushId( "Data" );
            DataSplitView.Draw();
        }
    }
}
