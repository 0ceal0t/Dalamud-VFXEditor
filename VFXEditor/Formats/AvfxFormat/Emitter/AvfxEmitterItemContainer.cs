using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterItemContainer : AvfxBase {
        public readonly bool IsParticle;
        public readonly AvfxEmitter Emitter;

        public readonly List<AvfxEmitterItem> Items = new();

        public AvfxEmitterItemContainer( string name, bool isParticle, AvfxEmitter emitter ) : base( name ) {
            IsParticle = isParticle;
            Emitter = emitter;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 312; i++ ) Items.Add( new AvfxEmitterItem( IsParticle, Emitter, false, reader ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) item.Write( writer );
        }
    }
}
