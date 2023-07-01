using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.CutbFormat.State {
    public class CutbInitialState : CutbHeader {
        public const string MAGIC = "CTIS";
        public override string Magic => MAGIC;

        public readonly ParsedString Scenario = new( "Scenario" );

        public CutbInitialState( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position;
            var offset = reader.ReadUInt32();

            ReadParsed( reader );

            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Scenario.Read( reader ); // Can be blank
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public override void Draw() {
            Scenario.Draw( CommandManager.Cutb );
            DrawParsed( CommandManager.Cutb );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            new ParsedByteBool( "Screen Fade Out" ),
            new ParsedByteBool( "BGM Fade Out" ),
            new ParsedByteBool( "Sounds Fade Out" ),
            new ParsedByteBool( "Can Skip" )
        };
    }
}
