using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXTextureNormal : Base {
        public LiteralBool Enabled = new( "bEna" );
        public LiteralInt UvSetIdx = new( "UvSN" );
        public LiteralEnum<TextureFilterType> TextureFilter = new( "TFT" );
        public LiteralEnum<TextureBorderType> TextureBorderU = new( "TBUT" );
        public LiteralEnum<TextureBorderType> TextureBorderV = new( "TBVT" );
        public LiteralInt TextureIdx = new( "TxNo" );
        public AVFXCurve NPow = new( "NPow" );
        private readonly List<Base> Attributes;

        public AVFXTextureNormal() : base( "TN" ) {
            Attributes = new List<Base>( new Base[]{
                Enabled,
                UvSetIdx,
                TextureFilter,
                TextureBorderU,
                TextureBorderV,
                TextureIdx,
                NPow
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetDefault( Attributes );
            SetDefault( NPow );
            NPow.AddKey();
            TextureIdx.GiveValue( -1 );
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( "TN" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
