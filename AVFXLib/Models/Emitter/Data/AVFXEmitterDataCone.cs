using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXEmitterDataCone : AVFXEmitterData {
        public LiteralEnum<RotationOrder> RotationOrderType = new( "ROT" );
        public AVFXCurve AngleY = new( "AnY" );
        public AVFXCurve OuterSize = new( "OuS" );
        public AVFXCurve InjectionSpeed = new( "IjS" );
        public AVFXCurve InjectionSpeedRandom = new( "IjSR" );
        public AVFXCurve InjectionAngle = new( "IjA" );
        private readonly List<Base> Attributes;

        public AVFXEmitterDataCone() : base( "Data" ) {
            Attributes = new List<Base>( new Base[] {
                RotationOrderType,
                AngleY,
                OuterSize,
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
