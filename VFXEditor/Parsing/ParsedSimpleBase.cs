namespace VfxEditor.Parsing {
    public abstract class ParsedSimpleBase<T> : ParsedBase {
        public T Value = default;
        public readonly string Name;

        public ParsedSimpleBase( string name ) {
            Name = name;
        }

        protected bool CopyPaste( CommandManager manager ) {
            var copy = manager.Copy;

            if( copy.IsCopying ) {
                copy.SetValue( this, Name, Value );
            }
            else if( copy.IsPasting && copy.GetValue<T>( this, Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<T>( this, val ) );
                return true;
            }

            return false;
        }
    }
}
