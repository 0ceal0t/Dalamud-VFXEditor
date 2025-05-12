using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataDisc : AvfxDataWithParameters {
        public readonly AvfxInt PartsCount = new( "Parts Count", "PrtC" );
        public readonly AvfxInt PartsCountU = new( "Parts Count U", "PCnU" );
        public readonly AvfxInt PartsCountV = new( "Parts Count V", "PCnV" );
        public readonly AvfxFloat PointIntervalFactoryV = new( "Point Interval Factor V", "PIFU" );
        public readonly AvfxCurve1Axis Angle = new( "Angle", "Ang", CurveType.Angle );
        public readonly AvfxCurve1Axis AngleRandom = new( "Angle Random", "AngR", CurveType.Angle );
        public readonly AvfxCurve1Axis HeightBeginInner = new( "Height Begin Inner", "HBI" );
        public readonly AvfxCurve1Axis HeightEndInner = new( "Height End Inner", "HEI" );
        public readonly AvfxCurve1Axis HeightBeginOuter = new( "Height Begin Outer", "HBO" );
        public readonly AvfxCurve1Axis HeightEndOuter = new( "Height End Outer", "HEO" );
        public readonly AvfxCurve1Axis WidthBegin = new( "Width Begin", "WB" );
        public readonly AvfxCurve1Axis WidthEnd = new( "Width End", "WE" );
        public readonly AvfxCurve1Axis RadiusBegin = new( "Radius Begin", "RB" );
        public readonly AvfxCurve1Axis RadiusEnd = new( "Radius End", "RE" );
        public readonly AvfxCurveColor ColorEdgeInner = new( name: "Color Edge Inner", "CEI" );
        public readonly AvfxCurveColor ColorEdgeOuter = new( name: "Color Edge Outer", "CEO" );
        public readonly AvfxInt SS = new( "Scaling Scale", "SS" );

        public AvfxParticleDataDisc() : base() {
            Parsed = [
                PartsCount,
                PartsCountU,
                PartsCountV,
                PointIntervalFactoryV,
                Angle,
                AngleRandom,
                HeightBeginInner,
                HeightEndInner,
                HeightBeginOuter,
                HeightEndOuter,
                WidthBegin,
                WidthEnd,
                RadiusBegin,
                RadiusEnd,
                ColorEdgeInner,
                ColorEdgeOuter,
                SS
            ];

            ParameterTab.Add( PartsCount );
            ParameterTab.Add( PartsCountU );
            ParameterTab.Add( PartsCountV );
            ParameterTab.Add( PointIntervalFactoryV );
            ParameterTab.Add( SS );

            Tabs.Add( Angle );
            Tabs.Add( AngleRandom );
            Tabs.Add( HeightBeginInner );
            Tabs.Add( HeightEndInner );
            Tabs.Add( HeightBeginOuter );
            Tabs.Add( HeightEndOuter );
            Tabs.Add( WidthBegin );
            Tabs.Add( WidthEnd );
            Tabs.Add( RadiusBegin );
            Tabs.Add( RadiusEnd );
            Tabs.Add( ColorEdgeInner );
            Tabs.Add( ColorEdgeOuter );
        }
    }
}
