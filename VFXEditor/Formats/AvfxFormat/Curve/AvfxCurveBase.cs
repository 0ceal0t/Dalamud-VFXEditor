using VfxEditor.AvfxFormat;

namespace VfxEditor.Formats.AvfxFormat.Curve {
    public abstract class AvfxCurveBase : AvfxOptional {
        public readonly string Name;

        protected AvfxCurveBase( string name, string avfxName, bool locked ) : base( avfxName, locked ) {
            Name = name;
        }

        public override string GetDefaultText() => Name;
    }
}
