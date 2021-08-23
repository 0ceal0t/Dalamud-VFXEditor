using AVFXLib.AVFX;
using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class LiteralFloat : LiteralBase {
        public float Value { get; set; }

        public LiteralFloat( string avfxName, int size = 4 ) : base( avfxName, size ) {
        }

        public override void Read( AVFXNode node ) {
        }

        public override void Read( AVFXLeaf leaf ) {
            Value = Util.Bytes4ToFloat( leaf.Contents );
            Size = leaf.Size;
            Assigned = true;
        }

        public void GiveValue( float value ) {
            Value = value;
            Assigned = true;
        }

        public override void ToDefault() {
            GiveValue( 0.0f );
        }

        public override AVFXNode ToAVFX() {
            return new AVFXLeaf( AVFXName, Size, Util.FloatTo4Bytes( Value ) );
        }

        public override string StringValue() {
            return Value.ToString();
        }
    }
}
