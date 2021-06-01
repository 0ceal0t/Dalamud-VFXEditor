using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXEmitterDataConeModel : AVFXEmitterData {
        public LiteralEnum<RotationOrder> RotationOrderType = new LiteralEnum<RotationOrder>( "ROT" );
        public LiteralEnum<GenerateMethod> GenerateMethodType = new LiteralEnum<GenerateMethod>( "GeMT" );
        public LiteralInt DivideX = new LiteralInt( "DivX" );
        public LiteralInt DivideY = new LiteralInt( "DivY" );
        public AVFXCurve AX = new AVFXCurve( "AnX" );
        public AVFXCurve AY = new AVFXCurve( "AnY" );
        public AVFXCurve Radius = new AVFXCurve( "Rad" );
        public AVFXCurve InjectionSpeed = new AVFXCurve( "IjS" );
        public AVFXCurve InjectionSpeedRandom = new AVFXCurve( "IjSR" );
        public AVFXCurve InjectionAngle = new AVFXCurve( "IjA" );

        List<Base> Attributes;

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
            AVFXNode dataAvfx = new AVFXNode( "Data" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
