using System;
using System.Collections.Generic;
using VfxEditor.FileManager;

namespace VfxEditor.Parsing.Data {
    public class ParsedDataListEnum<T, S> : ParsedEnum<T> where T : Enum where S : class {
        private readonly List<S> Items;

        public ParsedDataListEnum( List<S> items, string name, T value, int size = 4 ) : base( name, value, size ) {
            Items = items;
        }

        public ParsedDataListEnum( List<S> items, string name, int size = 4 ) : base( name, size ) {
            Items = items;
        }

        protected override void SetValue( T prevValue, T value ) {
            CommandManager.Add( new CompoundCommand( new ICommand[] {
                new ParsedSimpleCommand<T>( this, prevValue, value ),
                new GenericListCommand<S>( Items, new List<S>() )
            } ) );
        }

        protected override void SetValue( T value ) {
            CommandManager.Add( new CompoundCommand( new ICommand[] {
                new ParsedSimpleCommand<T>( this, value ),
                new GenericListCommand<S>( Items, new List<S>() )
            } ) );
        }
    }
}
