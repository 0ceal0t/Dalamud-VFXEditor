using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackAnalysisFlagData : ScdTrackData {
        public readonly List<short> Data = new();

        public override void Read( BinaryReader reader ) {
            var size = reader.ReadInt16();
            for( var i = 0; i < size; i++ ) Data.Add( reader.ReadInt16() );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( short )Data.Count );
            Data.ForEach( x => writer.Write( x ) );
        }

        public override void Draw( string parentId ) {
            
        }
    }
}
