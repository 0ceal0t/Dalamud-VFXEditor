using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEmitterDataCone : AVFXEmitterData
    {
        public LiteralEnum<RotationOrder> RotationOrderType = new LiteralEnum<RotationOrder>( "ROT" );
        public AVFXCurve AngleY = new AVFXCurve( "AnY" );
        public AVFXCurve OuterSize = new AVFXCurve( "OuS" );
        public AVFXCurve InjectionSpeed = new AVFXCurve( "IjS" );
        public AVFXCurve InjectionSpeedRandom = new AVFXCurve( "IjSR" );
        public AVFXCurve InjectionAngle = new AVFXCurve( "IjA" );

        List<Base> Attributes;

        public AVFXEmitterDataCone() : base( "Data" )
        {
            Attributes = new List<Base>( new Base[] {
                RotationOrderType,
                AngleY,
                OuterSize,
                InjectionSpeed,
                InjectionSpeedRandom,
                InjectionAngle
            } );
        }

        public override void read( AVFXNode node )
        {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void toDefault()
        {
            Assigned = true;
            SetUnAssigned( Attributes );
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode( "Data" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
