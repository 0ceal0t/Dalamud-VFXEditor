using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataDisc : UiData {
        public UiParameters Parameters;

        public UiParticleDataDisc( AVFXParticleDataDisc data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiInt( "Parts Count", data.PartsCount ) );
            Parameters.Add( new UiInt( "Parts Count U", data.PartsCountU ) );
            Parameters.Add( new UiInt( "Parts Count V", data.PartsCountV ) );
            Parameters.Add( new UiFloat( "Point Interval Factor V", data.PointIntervalFactoryV ) );
            Tabs.Add( new UiCurve( data.Angle, "Angle" ) );
            Tabs.Add( new UiCurve( data.HeightBeginInner, "Height Begin Inner" ) );
            Tabs.Add( new UiCurve( data.HeightEndInner, "Height End Inner" ) );
            Tabs.Add( new UiCurve( data.HeightBeginOuter, "Height Begin Outer" ) );
            Tabs.Add( new UiCurve( data.HeightEndOuter, "Height End Outer" ) );
            Tabs.Add( new UiCurve( data.WidthBegin, "Width Begin" ) );
            Tabs.Add( new UiCurve( data.WidthEnd, "Width End" ) );
            Tabs.Add( new UiCurve( data.RadiusBegin, "Radius Begin" ) );
            Tabs.Add( new UiCurve( data.RadiusEnd, "Radius End" ) );
            Tabs.Add( new UiCurveColor( data.ColorEdgeInner, "Color Edge Inner" ) );
            Tabs.Add( new UiCurveColor( data.ColorEdgeOuter, "Color Edge Outer" ) );
        }
    }
}
