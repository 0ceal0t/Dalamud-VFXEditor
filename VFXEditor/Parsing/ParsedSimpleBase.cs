using System.Collections.Generic;
using VfxEditor.Data;

namespace VfxEditor.Parsing {
    public abstract class ParsedSimpleBase<T> : ParsedBase {
        public T Value = default;
        public readonly string Name;

        public ParsedSimpleBase( string name ) {
            Name = name;
        }
    }

    public abstract class ParsedSimpleBase<T, C> : ParsedSimpleBase<T> {
        public ParsedSimpleBase( string name ) : base( name ) { }

        protected abstract Dictionary<string, C> GetCopyMap( CopyManager manager );

        protected abstract C ToCopy();

        protected abstract T FromCopy( C val );

        protected void Copy( CommandManager manager ) {
            var copy = manager.Copy;
            var copyMap = GetCopyMap( copy );

            if( copy.IsCopying ) {
                copyMap[Name] = ToCopy();
            }
            else if( copy.IsPasting && copyMap.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<T>( this, FromCopy( val ) ) );
            }
        }
    }
}
