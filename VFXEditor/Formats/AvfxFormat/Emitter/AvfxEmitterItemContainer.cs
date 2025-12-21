using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterItemContainer : AvfxBase {
        public readonly bool IsParticle;
        public readonly AvfxEmitter Emitter;

        private static readonly int[] ValidSizes = [312, 300, 288, 276];

        public readonly List<AvfxEmitterItem> Items = [];

        public AvfxEmitterItemContainer( string name, bool isParticle, AvfxEmitter emitter ) : base( name ) {
            IsParticle = isParticle;
            Emitter = emitter;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            foreach( var itemSize in ValidSizes ) {
                if( ( float )size / itemSize == size / itemSize ) {
                    for( var i = 0; i < size / itemSize; i++ ) Items.Add( new AvfxEmitterItem( IsParticle, Emitter, false, itemSize, reader ) );
                    return;
                }
            }
            Dalamud.Log( $"Size {size} cannot be parsed" );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) item.Write( writer );
        }
    }
}
