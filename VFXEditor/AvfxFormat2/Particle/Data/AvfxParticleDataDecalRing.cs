using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataDecalRing : AvfxData {
        public readonly AvfxCurve Width = new( "Width", "WID" );
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );
        public readonly AvfxFloat RingFan = new( "Ring Fan", "RF" );

        public readonly UiParameters Parameters;

        public AvfxParticleDataDecalRing() : base() {
            Children = new() {
                Width,
                ScalingScale,
                RingFan
            };

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( ScalingScale );
            Parameters.Add( RingFan );
            Tabs.Add( Width );
        }
    }
}
