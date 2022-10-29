using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public abstract class TmbEntry : TmbItemWithTime {
        public abstract string DisplayName { get; }

        public abstract void Draw( string id );

        public TmbEntry() : base() { }

        public TmbEntry( TmbReader reader ) : base( reader ) { }

        protected void DrawHeader( string id ) {
            FileUtils.ShortInput( $"Time{id}", ref Time );
        }
    }
}
