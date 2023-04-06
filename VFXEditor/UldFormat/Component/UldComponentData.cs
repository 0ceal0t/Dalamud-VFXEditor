using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
