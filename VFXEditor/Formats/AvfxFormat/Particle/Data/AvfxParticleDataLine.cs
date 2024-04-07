namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataLine : AvfxDataWithParameters {
        public readonly AvfxInt LineCount = new( "Line Count", "LnCT" );
        public readonly AvfxCurve Length = new( "Length", "Len" );
        public readonly AvfxCurve LengthRandom = new( "Length Random", "LenR" );
        public readonly AvfxCurveColor ColorBegin = new( name: "Color Begin", "ColB" );
        public readonly AvfxCurveColor ColorEnd = new( name: "Color End", "ColE" );

        public AvfxParticleDataLine() : base() {
            Parsed = [
                LineCount,
                Length,
                LengthRandom,
                ColorBegin,
                ColorEnd
            ];

            ParameterTab.Add( LineCount );

            DisplayTabs.Add( Length );
            DisplayTabs.Add( LengthRandom );
            DisplayTabs.Add( ColorBegin );
            DisplayTabs.Add( ColorEnd );
        }
    }
}
