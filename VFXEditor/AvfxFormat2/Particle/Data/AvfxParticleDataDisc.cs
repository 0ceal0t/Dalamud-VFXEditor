using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataDisc : AvfxData {
        public readonly AvfxInt PartsCount = new( "Parts Count", "PrtC" );
        public readonly AvfxInt PartsCountU = new( "Parts Count U", "PCnU" );
        public readonly AvfxInt PartsCountV = new( "Parts Count V", "PCnV" );
        public readonly AvfxFloat PointIntervalFactoryV = new( "Point Interval Factor V", "PIFU" );
        public readonly AvfxCurve Angle = new( "Angle", "Ang" );
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

        public readonly UiParameters Parameters;

        public AvfxParticleDataDisc() : base() {
            Children = new() {
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

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( PartsCount );
            Parameters.Add( PartsCountU );
            Parameters.Add( PartsCountV );
            Parameters.Add( PointIntervalFactoryV );
            Tabs.Add( Angle );
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
