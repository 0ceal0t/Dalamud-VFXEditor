using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataLine : AvfxDataWithParameters {
        public readonly AvfxInt LineCount = new( "Line Count", "LnCT" );
        public readonly AvfxCurve1Axis Length = new( "Length", "Len" );
        public readonly AvfxCurve1Axis LengthRandom = new( "Length Random", "LenR" );
        public readonly AvfxCurveColor ColorBegin;
        public readonly AvfxCurveColor ColorEnd;

        public AvfxParticleDataLine( AvfxFile file ) : base() {
            ColorBegin = new( file, name: "Color Begin", "ColB" );
            ColorEnd = new( file, name: "Color End", "ColE" );

            Parsed = [
                LineCount,
                Length,
                LengthRandom,
                ColorBegin,
                ColorEnd
            ];

            ParameterTab.Add( LineCount );

            Tabs.Add( Length );
            Tabs.Add( LengthRandom );
            Tabs.Add( ColorBegin );
            Tabs.Add( ColorEnd );
        }
    }
}
