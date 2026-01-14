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
        public readonly AvfxCurve1Axis HeightBeginInnerRandom = new( "Height Begin Inner Random", "HBIR" );
        public readonly AvfxCurve1Axis HeightEndInner = new( "Height End Inner", "HEI" );
        public readonly AvfxCurve1Axis HeightEndInnerRandom = new( "Height End Inner Random", "HEIR" );
        public readonly AvfxCurve1Axis HeightBeginOuter = new( "Height Begin Outer", "HBO" );
        public readonly AvfxCurve1Axis HeightBeginOuterRandom = new( "Height Begin Outer Random", "HBOR" );
        public readonly AvfxCurve1Axis HeightEndOuter = new( "Height End Outer", "HEO" );
        public readonly AvfxCurve1Axis HeightEndOuterRandom = new( "Height End Outer Random", "HEOR" );
        public readonly AvfxCurve1Axis WidthBegin = new( "Width Begin", "WB" );
        public readonly AvfxCurve1Axis WidthBeginRandom = new( "Width Begin Random", "WBR" );
        public readonly AvfxCurve1Axis WidthEnd = new( "Width End", "WE" );
        public readonly AvfxCurve1Axis WidthEndRandom = new( "Width End Random", "WER" );
        public readonly AvfxCurve1Axis RadiusBegin = new( "Radius Begin", "RB" );
        public readonly AvfxCurve1Axis RadiusBeginRandom = new( "Radius Begin Random", "RBR" );
        public readonly AvfxCurve1Axis RadiusEnd = new( "Radius End", "RE" );
        public readonly AvfxCurve1Axis RadiusEndRandom = new( "Radius End Random", "RER" );
        public readonly AvfxCurveColor ColorEdgeInner;
        public readonly AvfxCurveColor ColorEdgeOuter;
        public readonly AvfxInt SS = new( "Scaling Scale", "SS" );

        public AvfxParticleDataDisc( AvfxFile file ) : base() {
            ColorEdgeInner = new( file, name: "Color Edge Inner", "CEI" );
            ColorEdgeOuter = new( file, name: "Color Edge Outer", "CEO" );

            Parsed = [
                PartsCount,
                PartsCountU,
                PartsCountV,
                PointIntervalFactoryV,
                Angle,
                AngleRandom,
                HeightBeginInner,
                HeightBeginInnerRandom,
                HeightEndInner,
                HeightEndInnerRandom,
                HeightBeginOuter,
                HeightBeginOuterRandom,
                HeightEndOuter,
                HeightEndOuterRandom,
                WidthBegin,
                WidthBeginRandom,
                WidthEnd,
                WidthEndRandom,
                RadiusBegin,
                RadiusBeginRandom,
                RadiusEnd,
                RadiusEndRandom,
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
            Tabs.Add( HeightBeginInnerRandom );
            Tabs.Add( HeightEndInner );
            Tabs.Add( HeightEndInnerRandom );
            Tabs.Add( HeightBeginOuter );
            Tabs.Add( HeightBeginOuterRandom );
            Tabs.Add( HeightEndOuter );
            Tabs.Add( HeightEndOuterRandom );
            Tabs.Add( WidthBegin );
            Tabs.Add( WidthBeginRandom );
            Tabs.Add( WidthEnd );
            Tabs.Add( WidthEndRandom );
            Tabs.Add( RadiusBegin );
            Tabs.Add( RadiusBeginRandom );
            Tabs.Add( RadiusEnd );
            Tabs.Add( RadiusEndRandom );
            Tabs.Add( ColorEdgeInner );
            Tabs.Add( ColorEdgeOuter );
        }
    }
}
