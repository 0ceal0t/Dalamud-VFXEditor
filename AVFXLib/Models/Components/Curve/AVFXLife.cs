using AVFXLib.AVFX;
using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXLife : Base {
        public const string NAME = "Life";

        public bool Enabled;

        public LiteralFloat Value = new( "Val" );
        public LiteralFloat ValRandom = new( "ValR" );
        public LiteralEnum<RandomType> ValRandomType = new( "Type" );
        private readonly List<Base> Attributes;

        // Life is kinda strange, can either be -1 (4 bytes = ffffffff) or Val + ValR + RanT

        public AVFXLife() : base( NAME ) {
            Attributes = new List<Base>( new Base[]{
                Value,
                ValRandom,
                ValRandomType
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            Enabled = ( node.Children.Count > 2 );
            if( Enabled ) {
                ReadAVFX( Attributes, node );
            }
        }

        public override void ToDefault() {
            Assigned = true;
            Enabled = true;
            SetDefault( Attributes );
            Value.GiveValue( -1f );
        }

        public override AVFXNode ToAVFX() {
            if( Enabled ) {
                var lifeAvfx = new AVFXNode( "Life" );
                PutAVFX( lifeAvfx, Attributes );

                return lifeAvfx;

            }
            else // -1
            {
                AVFXNode lifeAvfx = new AVFXLeaf( "Life", 4, Util.IntTo4Bytes( -1 ) );
                return lifeAvfx;
            }
        }
    }
}
