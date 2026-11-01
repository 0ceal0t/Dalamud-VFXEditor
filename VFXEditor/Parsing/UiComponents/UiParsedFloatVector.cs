using System.Linq;
using System.Runtime.InteropServices;
using VfxEditor.Data.Command;
using VfxEditor.Data.Copy;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    // Edits several separate floats as a single Vector2/3/4
    public abstract class UiParsedFloatVector<T> : IUiItem where T : struct {
        public readonly string Name;
        public readonly ParsedFloat[] Components;
        public bool HighPrecision = true;

        protected string FloatFormat => HighPrecision ? UiUtils.HIGH_PRECISION_FORMAT : "%.3f";

        public T Value {
            get {
                var value = default( T );
                var span = MemoryMarshal.Cast<T, float>( MemoryMarshal.CreateSpan( ref value, 1 ) );
                for( var i = 0; i < span.Length; i++ ) span[i] = Components[i].Value;
                return value;
            }
        }

        protected UiParsedFloatVector( string name, params ParsedFloat[] components ) {
            Name = name;
            Components = components;
        }

        protected abstract bool DrawInput( ref T value );

        private CompoundCommand SetCommand( T value ) {
            var span = MemoryMarshal.Cast<T, float>( MemoryMarshal.CreateSpan( ref value, 1 ) ).ToArray();
            return new( Components.Select( ( x, i ) => ( ICommand )new ParsedSimpleCommand<float>( x, span[i] ) ).ToList() );
        }

        public void Draw() {
            // Copy/Paste
            CopyManager.TrySetValue( this, Name, Value );
            if( CopyManager.TryGetValue<T>( this, Name, out var val ) ) CommandManager.Paste( SetCommand( val ) );

            var value = Value;
            if( DrawInput( ref value ) ) CommandManager.Add( SetCommand( value ) );
        }
    }
}