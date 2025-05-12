using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedFlag<T> : ParsedSimpleBase<T> where T : Enum {
        private readonly int Size;
        private readonly bool ShowIntField;

        public int IntValue => ( int )( object )Value;

        public ParsedFlag( string name, T value, int size = 4 ) : base( name, value ) {
            Size = size;
        }

        public ParsedFlag( string name, int size = 4, bool showIntField = false ) : base( name ) {
            Size = size;
            ShowIntField = showIntField;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            var intValue = Size switch {
                4 => reader.ReadInt32(),
                2 => reader.ReadInt16(),
                1 => reader.ReadByte(),
                _ => reader.ReadByte()
            };
            Value = ( T )( object )intValue;
        }

        public override void Write( BinaryWriter writer ) {
            var intValue = Value == null ? -1 : IntValue;
            if( Size == 4 ) writer.Write( intValue );
            else if( Size == 2 ) writer.Write( ( short )intValue );
            else writer.Write( ( byte )intValue );
        }

        protected override void DrawBody() {
            if( ShowIntField ) {
                var value = IntValue;
                if( InTable ? ImGui.InputInt( Name, ref value, 0, 0 ) : ImGui.InputInt( Name, ref value ) ) {
                    Update( ( T )( object )value );
                }
            }

            var options = ( T[] )Enum.GetValues( typeof( T ) );
            foreach( var option in options ) {
                var intOption = ( int )( object )option;
                if( intOption == 0 ) continue;

                var hasFlag = HasFlag( option );
                if( ImGui.Checkbox( $"{option}".Replace( "_", " " ), ref hasFlag ) ) {
                    var intValue = IntValue;
                    if( hasFlag ) intValue |= intOption;
                    else intValue &= ~intOption;

                    Update( ( T )( object )intValue );
                }
            }
        }

        public void DrawWithIndent() {
            using var _ = ImRaii.PushId( Name );
            ImGui.TextDisabled( Name );
            using var indent = ImRaii.PushIndent( 10f );
            CopyPaste();
            DrawBody();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
        }

        public bool HasFlag( T option ) => Value.HasFlag( option );
    }
}
