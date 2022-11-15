using System;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataLine : AvfxData {
        public readonly AvfxInt LineCount = new( "Line Count", "LnCT" );
        public readonly AvfxCurve Length = new( "Length", "Len" );
        public readonly AvfxCurve LengthRandom = new( "Length Random", "LenR" );
        public readonly AvfxCurveColor ColorBegin = new( name: "Color Begin", "ColB" );
        public readonly AvfxCurveColor ColorEnd = new( name: "Color End", "ColE" );

        public readonly UiDisplayList Display;

        public AvfxParticleDataLine() : base() {
            Parsed = new() {
                LineCount,
                Length,
                LengthRandom,
                ColorBegin,
                ColorEnd
            };

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( LineCount );
            DisplayTabs.Add( Length );
            DisplayTabs.Add( LengthRandom );
            DisplayTabs.Add( ColorBegin );
            DisplayTabs.Add( ColorEnd );
        }
    }
}
