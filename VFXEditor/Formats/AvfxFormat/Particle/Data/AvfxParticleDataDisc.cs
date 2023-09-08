namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataDisc : AvfxData {
        public readonly AvfxInt PartsCount = new( "Parts Count", "PrtC" );
        public readonly AvfxInt PartsCountU = new( "Parts Count U", "PCnU" );
        public readonly AvfxInt PartsCountV = new( "Parts Count V", "PCnV" );
        public readonly AvfxFloat PointIntervalFactoryV = new( "Point Interval Factor V", "PIFU" );
        public readonly AvfxCurve Angle = new( "Angle", "Ang", CurveType.Angle );
        public readonly AvfxCurve HeightBeginInner = new( "Height Begin Inner", "HBI" );
        public readonly AvfxCurve HeightEndInner = new( "Height End Inner", "HEI" );
        public readonly AvfxCurve HeightBeginOuter = new( "Height Begin Outer", "HBO" );
        public readonly AvfxCurve HeightEndOuter = new( "Height End Outer", "HEO" );
        public readonly AvfxCurve WidthBegin = new( "Width Begin", "WB" );
        public readonly AvfxCurve WidthEnd = new( "Width End", "WE" );
        public readonly AvfxCurve RadiusBegin = new( "Radius Begin", "RB" );
        public readonly AvfxCurve RadiusEnd = new( "Radius End", "RE" );
        public readonly AvfxCurveColor ColorEdgeInner = new( name: "Color Edge Inner", "CEI" );
        public readonly AvfxCurveColor ColorEdgeOuter = new( name: "Color Edge Outer", "CEO" );

        public readonly UiDisplayList Display;

        public AvfxParticleDataDisc() : base() {
            Parsed = new() {
                PartsCount,
                PartsCountU,
                PartsCountV,
                PointIntervalFactoryV,
                Angle,
                HeightBeginInner,
                HeightEndInner,
                HeightBeginOuter,
                HeightEndOuter,
                WidthBegin,
                WidthEnd,
                RadiusBegin,
                RadiusEnd,
                ColorEdgeInner,
                ColorEdgeOuter
            };

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( PartsCount );
            Display.Add( PartsCountU );
            Display.Add( PartsCountV );
            Display.Add( PointIntervalFactoryV );
            DisplayTabs.Add( Angle );
            DisplayTabs.Add( HeightBeginInner );
            DisplayTabs.Add( HeightEndInner );
            DisplayTabs.Add( HeightBeginOuter );
            DisplayTabs.Add( HeightEndOuter );
            DisplayTabs.Add( WidthBegin );
            DisplayTabs.Add( WidthEnd );
            DisplayTabs.Add( RadiusBegin );
            DisplayTabs.Add( RadiusEnd );
            DisplayTabs.Add( ColorEdgeInner );
            DisplayTabs.Add( ColorEdgeOuter );
        }
    }
}
