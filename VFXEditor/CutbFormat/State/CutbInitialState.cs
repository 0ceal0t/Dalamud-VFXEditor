using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.CutbFormat.State {
    public class CutbInitialState : CutbHeader {
        public const string MAGIC = "CTIS";
        public override string Magic => MAGIC;

        public readonly ParsedString Scenario = new( "Scenario" );
        public readonly ParsedByteBool ScreenFadeOut = new( "Screen Fade Out" );
        public readonly ParsedByteBool BgmFadeOut = new( "BGM Fade Out" );
        public readonly ParsedByteBool SoundsFadeOut = new( "Sounds Fade Out" );
        public readonly ParsedByteBool CanSkip = new( "Can Skip" );

        public CutbInitialState( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position;
            var offset = reader.ReadUInt32();
            ScreenFadeOut.Read( reader );
            BgmFadeOut.Read( reader );
            SoundsFadeOut.Read( reader );
            CanSkip.Read( reader );

            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Scenario.Read( reader ); // Can be blank
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public override void Draw() {
            Scenario.Draw( CommandManager.Cutb );
            ScreenFadeOut.Draw( CommandManager.Cutb );
            BgmFadeOut.Draw( CommandManager.Cutb );
            SoundsFadeOut.Draw( CommandManager.Cutb );
            CanSkip.Draw( CommandManager.Cutb );
        }
    }
}
