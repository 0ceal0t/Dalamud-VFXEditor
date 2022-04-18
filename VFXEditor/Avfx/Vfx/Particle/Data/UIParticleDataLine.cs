using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.Avfx.Vfx {
    public class UIParticleDataLine : UIData {
        public UIParameters Parameters;

        public UIParticleDataLine( AVFXParticleDataLine data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UIInt( "Line Count", data.LineCount ) );
            Tabs.Add( new UICurve( data.Length, "Length" ) );
            Tabs.Add( new UICurve( data.LengthRandom, "Length Random" ) );
            Tabs.Add( new UICurveColor( data.ColorBegin, "Color Begin" ) );
            Tabs.Add( new UICurveColor( data.ColorEnd, "Color End" ) );
        }
    }
}
