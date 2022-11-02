using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataLine : UiData {
        public UiParameters Parameters;

        public UiParticleDataLine( AVFXParticleDataLine data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiInt( "Line Count", data.LineCount ) );
            Tabs.Add( new UiCurve( data.Length, "Length" ) );
            Tabs.Add( new UiCurve( data.LengthRandom, "Length Random" ) );
            Tabs.Add( new UiCurveColor( data.ColorBegin, "Color Begin" ) );
            Tabs.Add( new UiCurveColor( data.ColorEnd, "Color End" ) );
        }
    }
}
