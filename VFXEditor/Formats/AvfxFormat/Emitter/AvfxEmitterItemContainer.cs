using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterItemContainer : AvfxBase {
        public readonly bool IsParticle;
        public readonly AvfxEmitter Emitter;

        public readonly List<AvfxEmitterItem> Items = [];

        public AvfxEmitterItemContainer( string name, bool isParticle, AvfxEmitter emitter ) : base( name ) {
            IsParticle = isParticle;
            Emitter = emitter;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            if( ( float )size / 312 == size / 312 ) {
                for( var i = 0; i < size / 312; i++ ) Items.Add( new AvfxEmitterItem( IsParticle, Emitter, false, 312, reader ) );
            }
            else if( ( float )size / 300 == size / 300 ) {
                for( var i = 0; i < size / 300; i++ ) Items.Add( new AvfxEmitterItem( IsParticle, Emitter, false, 300, reader ) );
            }
            else if( ( float )size / 288 == size / 288 ) {
                for( var i = 0; i < size / 288; i++ ) Items.Add( new AvfxEmitterItem( IsParticle, Emitter, false, 288, reader ) );
            }
            else if( ( float )size / 276 == size / 276 ) {
                for( var i = 0; i < size / 276; i++ ) Items.Add( new AvfxEmitterItem( IsParticle, Emitter, false, 276, reader ) );
            }
            else
            {
                Dalamud.Log( "size " + size.ToString() + " cannot be parsed" );
            }
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) item.Write( writer );
        }
    }
}
