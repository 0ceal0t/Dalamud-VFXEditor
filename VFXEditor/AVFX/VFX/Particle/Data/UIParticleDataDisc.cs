using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AVFX.VFX {
    public class UIParticleDataDisc : UIData {
        public UIParameters Parameters;

        public UIParticleDataDisc( AVFXParticleDataDisc data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UIInt( "Parts Count", data.PartsCount ) );
            Parameters.Add( new UIInt( "Parts Count U", data.PartsCountU ) );
            Parameters.Add( new UIInt( "Parts Count V", data.PartsCountV ) );
            Parameters.Add( new UIFloat( "Point Interval Factor V", data.PointIntervalFactoryV ) );
            Tabs.Add( new UICurve( data.Angle, "Angle" ) );
            Tabs.Add( new UICurve( data.HeightBeginInner, "Height Begin Inner" ) );
            Tabs.Add( new UICurve( data.HeightEndInner, "Height End Inner" ) );
            Tabs.Add( new UICurve( data.HeightBeginOuter, "Height Begin Outer" ) );
            Tabs.Add( new UICurve( data.HeightEndOuter, "Height End Outer" ) );
            Tabs.Add( new UICurve( data.WidthBegin, "Width Begin" ) );
            Tabs.Add( new UICurve( data.WidthEnd, "Width End" ) );
            Tabs.Add( new UICurve( data.RadiusBegin, "Radius Begin" ) );
            Tabs.Add( new UICurve( data.RadiusEnd, "Radius End" ) );
            Tabs.Add( new UICurveColor( data.ColorEdgeInner, "Color Edge Inner" ) );
            Tabs.Add( new UICurveColor( data.ColorEdgeOuter, "Color Edge Outer" ) );
        }
    }
}
