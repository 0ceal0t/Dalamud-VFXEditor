using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurveKeys : AvfxBase {
        private readonly AvfxCurve Curve;
        public readonly List<AvfxCurveKey> Keys = new();

        public AvfxCurveKeys( AvfxCurve curve ) : base( "Keys" ) {
            Curve = curve;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var count = size / 16;
            for( var i = 0; i < count; i++ ) Keys.Add( new AvfxCurveKey( Curve, reader ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var key in Keys ) key.Write( writer );
        }
    }
}
