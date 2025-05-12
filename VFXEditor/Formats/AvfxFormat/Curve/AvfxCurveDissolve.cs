using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Curve;
using VfxEditor.Formats.AvfxFormat.Curve.Lines;
using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurveDissolve : AvfxCurveBase {
        public readonly AvfxCurveColor StartColor = new( "Start Color", "StrC" );
        public readonly AvfxCurveColor MidColor = new( "Mid Color", "MidC" );
        public readonly AvfxCurveColor EndColor = new( "End Color", "EndC" );
        public readonly AvfxCurveData SclR = new( "Scale R", "SclR" );
        public readonly AvfxCurveData SclG = new( "Scale G", "SclG" );
        public readonly AvfxCurveData SclB = new( "Scale B", "SclB" );
        public readonly AvfxCurveData Bri = new( "Brightness", "Bri" );

        private readonly List<AvfxItem> Parsed;

        private readonly List<AvfxItem> Display;

        public AvfxCurveDissolve( string name, string avfxName, bool locked = false ) : base( name, avfxName, locked ) {
            Parsed = [
                StartColor,
                MidColor,
                EndColor,
                SclR,
                SclG,
                SclB,
                Bri,
            ];

            Display = [
                StartColor,
                MidColor,
                EndColor,
                new LineEditor("Parameters", [
                    new("Scale", [SclR, SclG, SclB], null),
                    new(Bri)
                ])
            ];
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() {
            DrawNamedItems( Display );
        }
    }
}
