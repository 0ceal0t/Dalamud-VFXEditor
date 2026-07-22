using Dalamud.Bindings.ImGui;
using System;
using System.Linq;

namespace VfxEditor.Parsing
{
    public class ParsedShortByte2 : ParsedInt
    {
        public ParsedShortByte2( string name, int value ) : base( name, value, 2 ) { }

        public ParsedShortByte2( string name ) : base( name, size: 2 ) { }

        protected override void DrawBody()
        {
            var bytes = BitConverter.GetBytes( (short) Value );
            var value = bytes.Select( x => ( short )x ).ToArray();

            if( ImGui.InputShort( Name, value ) )
            {
                var newValue = BitConverter.ToInt16( value.Select( x => ( byte )x ).ToArray() );
                Update( newValue );
            }
        }
    }
}
