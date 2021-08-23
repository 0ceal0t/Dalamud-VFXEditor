using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXTextureReflection : Base {
        public LiteralBool Enabled = new( "bEna" );
        public LiteralBool UseScreenCopy = new( "bUSC" );
        public LiteralEnum<TextureFilterType> TextureFilter = new( "TFT" );
        public LiteralEnum<TextureCalculateColor> TextureCalculateColor = new( "TCCT" );
        public LiteralInt TextureIdx = new( "TxNo" );
        public AVFXCurve Rate = new( "Rate" );
        public AVFXCurve RPow = new( "RPow" );
        private readonly List<Base> Attributes;

        public AVFXTextureReflection() : base( "TR" ) {
            Attributes = new List<Base>( new Base[]{
                Enabled,
                UseScreenCopy,
                TextureFilter,
                TextureCalculateColor,
                TextureIdx,
                Rate,
                RPow
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetDefault( Attributes );
            SetUnAssigned( RPow );
            SetUnAssigned( Rate );
            TextureIdx.GiveValue( -1 );
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( "TR" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
