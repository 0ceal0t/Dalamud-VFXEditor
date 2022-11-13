using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataLine : AvfxData {
        public readonly AvfxInt LineCount = new( "Line Count", "LnCT" );
        public readonly AvfxCurve Length = new( "Length", "Len" );
        public readonly AvfxCurve LengthRandom = new( "Length Random", "LenR" );
        public readonly AvfxCurveColor ColorBegin = new( name: "Color Begin", "ColB" );
        public readonly AvfxCurveColor ColorEnd = new( name: "Color End", "ColE" );

        public readonly UiParameters Parameters;

        public AvfxParticleDataLine() : base() {
            Children = new() {
                LineCount,
                Length,
                LengthRandom,
                ColorBegin,
                ColorEnd
            };

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( LineCount );
            Tabs.Add( Length );
            Tabs.Add( LengthRandom );
            Tabs.Add( ColorBegin );
            Tabs.Add( ColorEnd );
        }
    }
}
