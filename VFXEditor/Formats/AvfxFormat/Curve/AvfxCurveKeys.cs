using System.Collections.Generic;
using System.IO;
using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurveKeys : AvfxBase {
        private readonly AvfxCurveData Curve;
        public readonly List<AvfxCurveKey> Keys = [];

        public AvfxCurveKeys( AvfxCurveData curve ) : base( "Keys" ) {
            Curve = curve;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var count = size / 16;
            for( var i = 0; i < count; i++ ) Keys.Add( new AvfxCurveKey( Curve, reader ) );
        }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var key in Keys ) key.Write( writer );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }
    }
}
