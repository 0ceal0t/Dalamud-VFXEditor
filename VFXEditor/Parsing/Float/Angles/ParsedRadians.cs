using OtterGui.Raii;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedRadians : ParsedFloat {
        public ParsedRadians( string name ) : base( name ) { }

        public ParsedRadians( string name, float value ) : base( name, value ) { }

        public override void Draw( CommandManager manager ) {
            using var _ = ImRaii.PushId( Name );
            Copy( manager );

            if( UiUtils.DrawRadians( Name, Value, out var newValue ) ) {
                manager.Add( new ParsedSimpleCommand<float>( this, newValue ) );
            }
        }
    }
}
