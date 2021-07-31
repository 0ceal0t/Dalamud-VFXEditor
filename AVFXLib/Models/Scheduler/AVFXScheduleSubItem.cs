using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {

    public class AVFXScheduleSubItem : Base {
        public LiteralBool Enabled = new( "bEna" );
        public LiteralInt StartTime = new( "StTm" );
        public LiteralInt TimelineIdx = new( "TlNo" );

        public List<Base> Attributes;

        public AVFXScheduleSubItem() : base( "SubItem" ) {
            Attributes = new List<Base>( new Base[]{
                Enabled,
                StartTime,
                TimelineIdx
            } );
        }

        public override void ToDefault() {
            Enabled.GiveValue( true );
            StartTime.GiveValue( 0 );
            TimelineIdx.GiveValue( -1 );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( "SubItem" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
