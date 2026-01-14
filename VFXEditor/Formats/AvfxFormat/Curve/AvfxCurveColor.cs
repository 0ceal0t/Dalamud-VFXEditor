using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Curve;
using VfxEditor.Formats.AvfxFormat.Curve.Lines;
using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurveColor : AvfxCurveBase {
        public readonly AvfxCurveData RGB = new( "RGB", "RGB", type: CurveType.Color );
        public readonly AvfxCurveData A = new( "A", "A" );
        public readonly AvfxCurveData SclR = new( "Scale R", "SclR" );
        public readonly AvfxCurveData SclG = new( "Scale G", "SclG" );
        public readonly AvfxCurveData SclB = new( "Scale B", "SclB" );
        public readonly AvfxCurveData SclA = new( "Scale A", "SclA" );
        public readonly AvfxCurveData Bri = new( "Brightness", "Bri" );
        public readonly AvfxCurveData RanR = new( "Random R", "RanR" );
        public readonly AvfxCurveData RanG = new( "Random G", "RanG" );
        public readonly AvfxCurveData RanB = new( "Random B", "RanB" );
        public readonly AvfxCurveData RanA = new( "Random A", "RanA" );
        public readonly AvfxCurveData RBri = new( "Random Bright", "RBri" );

        private readonly List<AvfxCurveBase> Parsed;

        private readonly LineEditor LineEditor;

        public AvfxCurveColor( AvfxFile file, string name, string avfxName = "Col", bool locked = false ) : base( name, avfxName, locked ) {
            Parsed = [
                RGB,
                A,
                SclR,
                SclG,
                SclB,
                SclA,
                Bri,
                RanR,
                RanG,
                RanB,
                RanA,
                RBri
            ];

            LineEditor = new( name, [
                new( RGB, file ),
                new( "Brightness", [A, Bri], null, file ),
                new( "Scale", [SclR, SclG, SclB, SclA], null, file ),
                new( "Random", [RanR, RanG, RanB, RanA, RBri], null, file )
            ] );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() {
            LineEditor.Draw();
        }
    }
}
