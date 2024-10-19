using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.AtchFormat.Entry;
using VfxEditor.Parsing.Utils;
using VfxEditor.Utils;

namespace VfxEditor.AtchFormat.Utils {
    public class AtchReader : ParsingReader {
        public long StartPosition;

        public AtchReader( BinaryReader reader ) : base( reader ) { }

        public void UpdateStartPosition()
        {
            StartPosition = Reader.BaseStream.Position;
        }

        public string ReadName() {  
            return FileUtils.Reverse( Reader.ReadString() );
        }

        public bool ReadAccessory() { 
            return Convert.ToBoolean( Reader.ReadInt32() );
        }

        public List<AtchEntryState> ReadOffsetState()
        {
            var offset = Reader.ReadInt32();
            var count = Reader.ReadInt32();
            var savePos = Reader.BaseStream.Position;
            var res = new List<AtchEntryState>();
            for( var i = 0; i < count; i++ )
            {
                string bone = Reader.ReadString();
                int scale = Reader.ReadInt32();
                Vector3 position = ReadVector3();
                Vector3 rotation = ReadVector3();
                res.Add( new AtchEntryState( bone, scale, position, rotation ) );
            }
            Reader.BaseStream.Position = savePos;
            return res;
        }

        public Vector3 ReadVector3()
        {
            var count = Reader.ReadInt32();

            if( count != 3 ) return new Vector3( 0 );

            var result = new Vector3()
            {
                X = Reader.ReadSingle(),
                Y = Reader.ReadSingle(),
                Z = Reader.ReadSingle(),
            };

            return result;
        }

        public static AtchEntry Import( byte[] data) {
            using var ms = new MemoryStream( data );
            using var reader = new BinaryReader( ms );
            var atchReader = new AtchReader( reader );

            return new AtchEntry( atchReader );
        }

    }
}
