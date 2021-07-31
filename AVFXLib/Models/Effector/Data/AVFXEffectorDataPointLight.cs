using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXEffectorDataPointLight : AVFXEffectorData {
        public AVFXCurveColor Color = new();
        public AVFXCurve DistanceScale = new( "DstS" );
        public AVFXCurve3Axis Rotation = new( "Rot" );
        public AVFXCurve3Axis Position = new( "Pos" );
        public LiteralEnum<PointLightAttenuation> PointLightAttenuationType = new( "Attn" );
        public LiteralBool EnableShadow = new( "bSdw" );
        public LiteralBool EnableCharShadow = new( "bChS" );
        public LiteralBool EnableMapShadow = new( "bMpS" );
        public LiteralBool EnableMoveShadow = new( "bMvS" );
        public LiteralFloat ShadowCreateDistanceNear = new( "SCDN" );
        public LiteralFloat ShadowCreateDistanceFar = new( "SCDF" );
        private readonly List<Base> Attributes;

        public AVFXEffectorDataPointLight() : base( "Data" ) {
            Attributes = new List<Base>( new Base[]{
                Color,
                DistanceScale,
                Rotation,
                Position,
                PointLightAttenuationType,
                EnableShadow,
                EnableCharShadow,
                EnableMapShadow,
                EnableMoveShadow,
                ShadowCreateDistanceNear,
                ShadowCreateDistanceFar
            } );
        }

        public override void ToDefault() {
            Assigned = true;
            SetDefault( Attributes );
            SetUnAssigned( Color );
            SetUnAssigned( DistanceScale );
            SetUnAssigned( Rotation );
            SetUnAssigned( Position );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( "Data" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
