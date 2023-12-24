using Dalamud.Interface.Utility.Raii;
using System;
using VfxEditor.Data.Copy;

namespace VfxEditor.Parsing {
    public abstract class ParsedSimpleBase<T> : ParsedBase {
        public readonly string Name;

        protected bool InTable => Name.StartsWith( "##" );

        public T Value = default;
        public Action OnChangeAction;

        public ParsedSimpleBase( string name, T value ) : this( name ) {
            Value = value;
        }

        public ParsedSimpleBase( string name ) {
            Name = name;
        }

        protected bool CopyPaste() {
            CopyManager.TrySetValue( this, Name, Value );
            if( CopyManager.TryGetValue<T>( this, Name, out var val ) ) {
                CommandManager.Paste( new ParsedSimpleCommand<T>( this, val, OnChangeAction ) );
                return true;
            }
            return false;
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( Name );
            CopyPaste();
            DrawBody();
        }

        protected abstract void DrawBody();

        public virtual void Update( T prevValue, T value ) {
            CommandManager.Add( new ParsedSimpleCommand<T>( this, prevValue, value, OnChangeAction ) );
        }

        public virtual void Update( T value ) {
            CommandManager.Add( new ParsedSimpleCommand<T>( this, value, OnChangeAction ) );
        }
    }
}
