using OtterGui.Raii;
using System;

namespace VfxEditor.Parsing {
    public abstract class ParsedSimpleBase<T> : ParsedBase {
        public readonly string Name;

        public T Value = default;
        public Action OnChangeAction;

        public ParsedSimpleBase( string name, T value ) : this( name ) {
            Value = value;
        }

        public ParsedSimpleBase( string name ) {
            Name = name;
        }

        protected bool CopyPaste() {
            /*
            var copy = manager.Copy;

            // TODO
            if( copy.IsCopying ) {
                copy.SetValue( this, Name, Value );
            }
            else if( copy.IsPasting && copy.GetValue<T>( this, Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<T>( this, val, OnChangeAction ) );
                return true;
            }
            */
            return false;
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( Name );
            CopyPaste();
            DrawBody();
        }

        protected abstract void DrawBody();

        protected virtual void SetValue( T prevValue, T value ) {
            CommandManager.Add( new ParsedSimpleCommand<T>( this, prevValue, value, OnChangeAction ) );
        }

        protected virtual void SetValue( T value ) {
            CommandManager.Add( new ParsedSimpleCommand<T>( this, value, OnChangeAction ) );
        }
    }
}
