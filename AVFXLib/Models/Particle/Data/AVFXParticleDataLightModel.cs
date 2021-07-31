using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXParticleDataLightModel : AVFXParticleData {
        public LiteralInt ModelIdx = new( "MNO", size: 1 );
        private readonly List<Base> Attributes;

        public AVFXParticleDataLightModel() : base( "Data" ) {
            Attributes = new List<Base>( new Base[]{
                ModelIdx
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetDefault( Attributes );
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( "Data" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
