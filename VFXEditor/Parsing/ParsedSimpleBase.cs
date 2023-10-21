using OtterGui.Raii;
using System;

namespace VfxEditor.Parsing {
    public abstract class ParsedSimpleBase<T> : ParsedBase {
        public readonly string Name;

        public T Value = default;
        public Func<ICommand> Extra;

        public ParsedSimpleBase( string name, T value ) : this( name ) {
            Value = value;
        }

        public ParsedSimpleBase( string name ) {
            Name = name;
        }

        protected bool CopyPaste( CommandManager manager ) {
            var copy = manager.Copy;

            if( copy.IsCopying ) {
                copy.SetValue( this, Name, Value );
            }
            else if( copy.IsPasting && copy.GetValue<T>( this, Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<T>( this, val, Extra?.Invoke() ) );
                return true;
            }

            return false;
        }

        public override void Draw( CommandManager manager ) {
            using var _ = ImRaii.PushId( Name );
            CopyPaste( manager );
            DrawBody( manager );
        }

        protected abstract void DrawBody( CommandManager manager );

        protected void SetValue( CommandManager manager, T prevValue, T value ) {
            manager.Add( new ParsedSimpleCommand<T>( this, prevValue, value, Extra?.Invoke() ) );
        }

        protected void SetValue( CommandManager manager, T value ) {
            manager.Add( new ParsedSimpleCommand<T>( this, value, Extra?.Invoke() ) );
        }
    }
}
