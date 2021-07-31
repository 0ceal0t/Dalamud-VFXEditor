using AVFXLib.AVFX;
using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class LiteralString : LiteralBase {
        public string Value { get; set; }
        public int FixedSize;

        public LiteralString( string avfxName, int size = 4, int fixedSize = -1 ) : base( avfxName, size ) {
            FixedSize = fixedSize;
        }

        public override void Read( AVFXNode node ) {
        }

        public override void Read( AVFXLeaf leaf ) {
            Value = Util.BytesToString( leaf.Contents );
            if( FixedSize == -1 )
                Size = leaf.Size;
            else
                Size = FixedSize;
            Assigned = true;
        }

        public void GiveValue( string value ) {
            Value = value;
            if( FixedSize == -1 )
                Size = Value.Length;
            else
                Size = FixedSize;
            Assigned = true;
        }

        public override void ToDefault() {
            GiveValue( "" );
        }

        public override AVFXNode ToAVFX() {
            return new AVFXLeaf( AVFXName, Size, Util.StringToBytes( Value ) );
        }

        public override string StringValue() {
            return Value.ToString();
        }
    }
}
