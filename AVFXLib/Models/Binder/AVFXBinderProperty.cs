using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXBinderProperty : Base {
        public string Name = "PrpS";

        public LiteralEnum<BindPoint> BindPointType = new( "BPT" );
        public LiteralEnum<BindTargetPoint> BindTargetPointType = new( "BPTP" );
        public LiteralString BinderName = new( "Name" );
        public LiteralInt BindPointId = new( "BPID" );
        public LiteralInt GenerateDelay = new( "GenD" );
        public LiteralInt CoordUpdateFrame = new( "CoUF" );
        public LiteralBool RingEnable = new( "bRng" );
        public LiteralInt RingProgressTime = new( "RnPT" );
        public LiteralFloat RingPositionX = new( "RnPX" );
        public LiteralFloat RingPositionY = new( "RnPY" );
        public LiteralFloat RingPositionZ = new( "RnPZ" );
        public LiteralFloat RingRadius = new( "RnRd" );
        public AVFXCurve3Axis Position = new( "Pos" );
        private readonly List<Base> Attributes;

        public AVFXBinderProperty( string name ) : base( name ) {
            Name = name;
            Attributes = new List<Base>( new Base[]{
                BindPointType,
                BindTargetPointType,
                BinderName,
                BindPointId,
                GenerateDelay,
                CoordUpdateFrame,
                RingEnable,
                RingProgressTime,
                RingPositionX,
                RingPositionY,
                RingPositionZ,
                RingRadius,
                Position
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetDefault( Attributes );
            BinderName.Assigned = false;
            BinderName.GiveValue( "" );

            BindTargetPointType.GiveValue( BindTargetPoint.ByName );
            BindPointId.GiveValue( 3 );
            CoordUpdateFrame.GiveValue( -1 );
            RingProgressTime.GiveValue( 1 );
            Position.ToDefault();
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( Name );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
