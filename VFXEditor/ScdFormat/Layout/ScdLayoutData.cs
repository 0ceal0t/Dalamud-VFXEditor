using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class ScdLayoutData {
        public List<ParsedBase> Parsed;

        public void Read( BinaryReader reader ) => Parsed.ForEach( x => x.Read( reader ) );

        public void Write( BinaryWriter writer ) => Parsed.ForEach( x => x.Write( writer ) );

        public void Draw( string parentId ) => Parsed.ForEach( x => x.Draw( parentId, CommandManager.Scd ) );
    }
}
