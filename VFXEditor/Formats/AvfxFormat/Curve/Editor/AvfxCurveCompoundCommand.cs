using System.Collections.Generic;
using VfxEditor.AvfxFormat;

namespace VfxEditor.Formats.AvfxFormat.Curve.Editor {
    public class AvfxCurveCompoundCommand : CompoundCommand {
        private readonly AvfxCurve Curve;

        public AvfxCurveCompoundCommand( AvfxCurve curve ) {
            Curve = curve;
        }

        public AvfxCurveCompoundCommand( AvfxCurve curve, IEnumerable<ICommand> commands ) : base( commands ) {
            Curve = curve;
        }

        public override void Execute() {
            base.Execute();
            Curve.Update();
        }

        public override void Redo() {
            base.Redo();
            Curve.Update();
        }

        public override void Undo() {
            base.Undo();
            Curve.Update();
        }
    }
}
