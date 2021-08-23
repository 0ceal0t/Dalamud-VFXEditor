using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXEmitterDataSphereModel : AVFXEmitterData {
        public LiteralEnum<RotationOrder> RotationOrderType = new( "ROT" );
        public LiteralEnum<GenerateMethod> GenerateMethodType = new( "GeMT" );
        public LiteralInt DivideX = new( "DivX" );
        public LiteralInt DivideY = new( "DivY" );
        public AVFXCurve Radius = new( "Rads" );
        public AVFXCurve InjectionSpeed = new( "IjS" );
        public AVFXCurve InjectionSpeedRandom = new( "IjSR" );
        private readonly List<Base> Attributes;

        public AVFXEmitterDataSphereModel() : base( "Data" ) {
            Attributes = new List<Base>( new Base[] {
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                Radius,
                InjectionSpeed,
                InjectionSpeedRandom
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetUnAssigned( Attributes );
            SetDefault( RotationOrderType );
            SetDefault( GenerateMethodType );
            DivideX.GiveValue( 1 );
            DivideY.GiveValue( 1 );
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( "Data" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}