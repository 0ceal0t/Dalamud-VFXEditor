using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXEmitterDataConeModel : AVFXEmitterData {
        public LiteralEnum<RotationOrder> RotationOrderType = new( "ROT" );
        public LiteralEnum<GenerateMethod> GenerateMethodType = new( "GeMT" );
        public LiteralInt DivideX = new( "DivX" );
        public LiteralInt DivideY = new( "DivY" );
        public AVFXCurve AX = new( "AnX" );
        public AVFXCurve AY = new( "AnY" );
        public AVFXCurve Radius = new( "Rad" );
        public AVFXCurve InjectionSpeed = new( "IjS" );
        public AVFXCurve InjectionSpeedRandom = new( "IjSR" );
        public AVFXCurve InjectionAngle = new( "IjA" );
        private readonly List<Base> Attributes;

        public AVFXEmitterDataConeModel() : base( "Data" ) {
            Attributes = new List<Base>( new Base[] {
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                AX,
                AY,
                Radius,
                InjectionSpeed,
                InjectionSpeedRandom,
                InjectionAngle
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetUnAssigned( Attributes );
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( "Data" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
