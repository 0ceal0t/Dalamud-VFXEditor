using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using System.Collections.Generic;
using VfxEditor.Parsing;
using System.Numerics;

namespace VfxEditor.TmbFormat.Entries {
    public enum DangerLevel : int {
        None,
        Yellow,
        Red,
        Detectable,
        DontAddRemove
    }

    public abstract class TmbEntry : TmbItemWithTime {
        public abstract string DisplayName { get; }
        public virtual DangerLevel Danger => DangerLevel.None;

        private readonly List<ParsedBase> Parsed;

        public TmbEntry( bool papEmbedded ) : base( papEmbedded ) {
            Parsed = GetParsed();
        }

        public TmbEntry( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            Parsed = GetParsed();
        }

        public virtual void Draw() {
            DrawHeader();
            DrawParsed();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            WriteParsed( writer );
        }

        protected void ReadParsed( TmbReader reader ) {
            foreach( var item in Parsed ) item.Read( reader );
        }

        protected void WriteParsed( TmbWriter writer ) {
            foreach( var item in Parsed ) item.Write( writer );
        }

        protected void DrawParsed() {
            foreach( var item in Parsed ) item.Draw( Command );
        }

        protected abstract List<ParsedBase> GetParsed();

        public static bool DoColor( DangerLevel level, out Vector4 color ) {
            color = new( 1 );
            if( level < DangerLevel.Yellow ) return false;
            else if( level == DangerLevel.Yellow ) color = UiUtils.YELLOW_COLOR;
            else color = UiUtils.RED_COLOR;

            return true;
        }
    }
}
