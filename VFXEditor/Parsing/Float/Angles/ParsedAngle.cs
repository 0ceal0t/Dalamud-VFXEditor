using OtterGui.Raii;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedAngle : ParsedFloat {
        public ParsedAngle( string name ) : base( name ) { }

        public ParsedAngle( string name, float defaultValue ) : base( name, defaultValue ) { }

        public override void Draw( CommandManager manager ) {
            using var _ = ImRaii.PushId( Name );
            Copy( manager );

            if( UiUtils.DrawAngle( Name, Value, out var newValue ) ) {
                manager.Add( new ParsedSimpleCommand<float>( this, newValue ) );
            }
        }
    }
}
