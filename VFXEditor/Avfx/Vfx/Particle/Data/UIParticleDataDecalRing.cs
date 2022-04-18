using System;
using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.Avfx.Vfx {
    public class UIParticleDataDecalRing : UIData {
        public UIParameters Parameters;

        public UIParticleDataDecalRing( AVFXParticleDataDecalRing data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UIFloat( "Scaling Scale", data.ScalingScale ) );
            Parameters.Add( new UIFloat( "Ring Fan", data.RingFan ) );
            Tabs.Add( new UICurve( data.Width, "Width" ) );
        }
    }
}
