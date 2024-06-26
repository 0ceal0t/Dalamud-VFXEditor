using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurveColor : AvfxCurveBase {
        public readonly AvfxCurve RGB = new( "RGB", "RGB", type: CurveType.Color );
        public readonly AvfxCurve A = new( "A", "A" );
        public readonly AvfxCurve SclR = new( "Scale R", "SclR" );
        public readonly AvfxCurve SclG = new( "Scale G", "SclG" );
        public readonly AvfxCurve SclB = new( "Scale B", "SclB" );
        public readonly AvfxCurve SclA = new( "Scale A", "SclA" );
        public readonly AvfxCurve Bri = new( "Brightness", "Bri" );
        public readonly AvfxCurve RanR = new( "Random R", "RanR" );
        public readonly AvfxCurve RanG = new( "Random G", "RanG" );
        public readonly AvfxCurve RanB = new( "Random B", "RanB" );
        public readonly AvfxCurve RanA = new( "Random A", "RanA" );
        public readonly AvfxCurve RBri = new( "Random Bright", "RBri" );

        private readonly List<AvfxCurveBase> Curves;

        public AvfxCurveColor( string name, string avfxName = "Col", bool locked = false ) : base( name, avfxName, locked ) {
            Curves = [
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
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Curves, size );

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Curves );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Curves ) yield return item;
        }

        protected override void DrawBody() {
            DrawUnassignedCurves( Curves );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawAssignedCurves( Curves );
        }
    }
}
