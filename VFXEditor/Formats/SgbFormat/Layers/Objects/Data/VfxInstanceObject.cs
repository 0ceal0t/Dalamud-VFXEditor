using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class VfxInstanceObject : SgbObject {
        private readonly ParsedString Path = new( "Path" );
        private readonly ParsedFloat SoftParticleFadeRange = new( "Soft Particle Fade Range" );
        private readonly ParsedIntColor Color = new( "Color" );
        private readonly ParsedByteBool AutoPlay = new( "Auto Play" );
        private readonly ParsedByteBool NoFarClip = new( "No Far Clip" );
        private readonly ParsedFloat2 FadeNear = new( "Fade Near" );
        private readonly ParsedFloat2 FadeFar = new( "Fade Far" );
        private readonly ParsedFloat ZCorrect = new( "Z Correct" );

        public VfxInstanceObject( LayerEntryType type ) : base( type ) { }

        public VfxInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {
            Path.Draw();
            SoftParticleFadeRange.Draw();
            Color.Draw();
            AutoPlay.Draw();
            NoFarClip.Draw();
            FadeNear.Draw();
            FadeFar.Draw();
            ZCorrect.Draw();
        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {
            Path.Value = FileUtils.ReadStringOffset( startPos, reader );
            SoftParticleFadeRange.Read( reader );
            reader.ReadBytes( 4 ); // padding
            Color.Read( reader );
            AutoPlay.Read( reader );
            NoFarClip.Read( reader );
            reader.ReadBytes( 2 ); // padding
            FadeNear.Read( reader );
            FadeFar.Read( reader );
            ZCorrect.Read( reader );
        }
    }
}
