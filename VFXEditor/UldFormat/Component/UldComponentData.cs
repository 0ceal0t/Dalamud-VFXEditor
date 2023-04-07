using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component {
    public abstract class UldComponentData {
        protected List<ParsedBase> Parsed = new();

        public virtual void Read( BinaryReader reader ) {
            foreach( var parsed in Parsed ) parsed.Read( reader );
        }

        public virtual void Write( BinaryWriter writer ) {
            foreach( var parsed in Parsed ) parsed.Write( writer );
        }

        public virtual void Draw( string id ) {
            foreach( var parsed in Parsed ) parsed.Draw( id, CommandManager.Uld );
        }

        protected void AddUnknown( int count ) {
            for( var i = 1; i <= ( count + 1 ); i++ ) Parsed.Add( new ParsedUInt( $"Unknown {i}" ) );
        }
    }
}
