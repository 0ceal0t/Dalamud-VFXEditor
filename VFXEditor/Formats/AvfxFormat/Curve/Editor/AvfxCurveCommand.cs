using VfxEditor.AvfxFormat;

namespace VfxEditor.Formats.AvfxFormat.Curve.Editor {
    public class AvfxCurveCommand : ICommand {
        private readonly AvfxCurve Curve;

        public AvfxCurveCommand( AvfxCurve curve ) {
            Curve = curve;
        }

        public void Execute() => Curve.Update();

        public void Redo() => Curve.Update();

        public void Undo() => Curve.Update();
    }
}
