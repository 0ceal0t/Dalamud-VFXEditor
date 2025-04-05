using Dalamud.Interface.Utility.Raii;
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

        public override void Draw() {
            if( IsAssigned() ) {
                using var _ = ImRaii.PushId( GetDefaultText() );
                DrawBody();
            }
            else {
                // Don't want to show unassign button b/c it can already be done with the checkbox
                using var _ = ImRaii.PushId( GetDefaultText() );
                AssignedCopyPaste( GetDefaultText() );
                DrawAssignButton( GetDefaultText(), true );
            }
        }
    }
}
