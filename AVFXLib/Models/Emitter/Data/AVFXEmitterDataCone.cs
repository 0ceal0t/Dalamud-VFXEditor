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
        public LiteralEnum<RotationOrder> RotationOrderType = new LiteralEnum<RotationOrder>( "rotationOrder", "ROT" );
        public AVFXCurve AngleY = new AVFXCurve( "angleY", "AnY" );
        public AVFXCurve OuterSize = new AVFXCurve( "outerSize", "OuS" );
        public AVFXCurve InjectionSpeed = new AVFXCurve( "injectionSpeed", "IjS" );
        public AVFXCurve InjectionSpeedRandom = new AVFXCurve( "injectionSpeedRandom", "IjSR" );
        public AVFXCurve InjectionAngle = new AVFXCurve( "injectionAngle", "IjA" );

        List<Base> Attributes;

        public AVFXEmitterDataCone( string jsonPath ) : base( jsonPath, "Data" )
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

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON( elem, Attributes );
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode( "Data" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
