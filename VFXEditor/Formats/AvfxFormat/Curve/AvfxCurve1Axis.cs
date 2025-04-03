using VfxEditor.Formats.AvfxFormat.Curve.Lines;

namespace VFXEditor.Formats.AvfxFormat.Curve {
    public class AvfxCurve1Axis : AvfxCurveData {
        private readonly LineEditor LineEditor;

        public AvfxCurve1Axis( string name, string avfxName, CurveType type = CurveType.Base, bool locked = false ) : base( name, avfxName, type, locked ) {
            LineEditor = new( this );
        }

        public override void DrawBody() {
            LineEditor.Draw();
        }
    }
}
