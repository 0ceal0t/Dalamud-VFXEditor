using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurveDissolve : AvfxCurveBase {
        public readonly AvfxCurveColor StartColor = new( "Start Color", "StrC" );
        public readonly AvfxCurveColor MidColor = new( "Mid Color", "MidC" );
        public readonly AvfxCurveColor EndColor = new( "End Color", "EndC" );
        public readonly AvfxCurve SclR = new( "Scale R", "SclR" );
        public readonly AvfxCurve SclG = new( "Scale G", "SclG" );
        public readonly AvfxCurve SclB = new( "Scale B", "SclB" );
        public readonly AvfxCurve Bri = new( "Brightness", "Bri" );

        private readonly List<AvfxCurveBase> Curves;

        public AvfxCurveDissolve( string name, string avfxName, bool locked = false ) : base( name, avfxName, locked ) {
            Curves = [
                StartColor,
                MidColor,
                EndColor,
                SclR,
                SclG,
                SclB,
                Bri,
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
