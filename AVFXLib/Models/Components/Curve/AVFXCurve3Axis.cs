using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXCurve3Axis : Base {
        public LiteralEnum<AxisConnect> AxisConnectType = new( "ACT" );
        public LiteralEnum<RandomType> AxisConnectRandomType = new( "ACTR" );
        public AVFXCurve X = new( "X" );
        public AVFXCurve Y = new( "Y" );
        public AVFXCurve Z = new( "Z" );
        public AVFXCurve RX = new( "XR" );
        public AVFXCurve RY = new( "YR" );
        public AVFXCurve RZ = new( "ZR" );
        private readonly List<Base> Attributes;

        public AVFXCurve3Axis( string avfxName ) : base( avfxName ) {
            Attributes = new List<Base>( new Base[]{
                AxisConnectType,
                AxisConnectRandomType,
                X,
                Y,
                Z,
                RX,
                RY,
                RZ
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetUnAssigned( Attributes );
            SetDefault( AxisConnectType );
            SetDefault( AxisConnectRandomType );
        }

        public override AVFXNode ToAVFX() {
            var curveAvfx = new AVFXNode( AVFXName );
            PutAVFX( curveAvfx, Attributes );
            return curveAvfx;
        }
    }
}
