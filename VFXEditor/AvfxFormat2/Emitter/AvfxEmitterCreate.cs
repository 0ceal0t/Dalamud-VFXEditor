using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitterCreate : AvfxBase {
        public readonly bool IsParticle;
        public readonly AvfxEmitter Emitter;

        public readonly List<AvfxEmitterItem> Items = new();

        public AvfxEmitterCreate( string name, bool isParticle, AvfxEmitter emitter ) : base( name ) {
            IsParticle = isParticle;
            Emitter = emitter;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 312; i++ ) Items.Add( new AvfxEmitterItem( IsParticle, Emitter, reader ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) item.Write( writer );
        }
    }
}
