using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXParticleUVSet : Base {
        public const string NAME = "UvSt";

        public LiteralEnum<TextureCalculateUV> CalculateUVType = new( "CUvT" );
        public AVFXCurve2Axis Scale = new( "Scl" );
        public AVFXCurve2Axis Scroll = new( "Scr" );
        public AVFXCurve Rot = new( "Rot" );
        public AVFXCurve RotRandom = new( "RotR" );
        private readonly List<Base> Attributes;

        public AVFXParticleUVSet() : base( NAME ) {
            Attributes = new List<Base>( new Base[] {
                CalculateUVType,
                Scale,
                Scroll,
                Rot,
                RotRandom
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetUnAssigned( Attributes );
            SetDefault( CalculateUVType );
        }

        public override AVFXNode ToAVFX() {
            var uvstAvfx = new AVFXNode( "UvSt" );
            PutAVFX( uvstAvfx, Attributes );
            return uvstAvfx;
        }
    }
}
