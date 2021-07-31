using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXParticleDataDisc : AVFXParticleData {
        public LiteralInt PartsCount = new( "PrtC" );
        public LiteralInt PartsCountU = new( "PCnU" );
        public LiteralInt PartsCountV = new( "PCnV" );
        public LiteralFloat PointIntervalFactoryV = new( "PIFU" );

        public AVFXCurve Angle = new( "Ang" );
        public AVFXCurve HeightBeginInner = new( "HBI" );
        public AVFXCurve HeightEndInner = new( "HEI" );
        public AVFXCurve HeightBeginOuter = new( "HBO" );
        public AVFXCurve HeightEndOuter = new( "HEO" );
        public AVFXCurve WidthBegin = new( "WB" );
        public AVFXCurve WidthEnd = new( "WE" );
        public AVFXCurve RadiusBegin = new( "RB" );
        public AVFXCurve RadiusEnd = new( "RE" );
        public AVFXCurveColor ColorEdgeInner = new( name: "CEI" );
        public AVFXCurveColor ColorEdgeOuter = new( name: "CEO" );
        private readonly List<Base> Attributes;

        public AVFXParticleDataDisc() : base( "Data" ) {
            Attributes = new List<Base>( new Base[]{
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
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetDefault( Attributes );
            SetUnAssigned( Angle );
            SetUnAssigned( WidthBegin );
            SetUnAssigned( WidthEnd );
            SetUnAssigned( RadiusBegin );
            SetUnAssigned( RadiusEnd );
            SetUnAssigned( ColorEdgeInner );
            SetUnAssigned( ColorEdgeOuter );
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( "Data" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
