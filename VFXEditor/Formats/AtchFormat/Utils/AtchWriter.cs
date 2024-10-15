using System;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.AtchFormat.Entry;
using VfxEditor.Parsing.Utils;
using VfxEditor.Utils;

namespace VfxEditor.AtchFormat.Utils {
    public class AtchWriter : ParsingWriter {
        public int BodySize;
        public int StateSize;

        public long StartPosition;
        public readonly BinaryWriter StateWriter;

        public readonly MemoryStream StateMs;

        public AtchWriter( int bodySize, int stateSize ) : base() {
            BodySize = bodySize;
            StateSize = stateSize;

            StateMs = new();
            StateWriter = new( StateMs );
        }

        public override void Dispose()
        {
            base.Dispose();
            StateWriter.Close();
            StateMs.Close();
        }

        public void WriteEntry( AtchEntry entry )
        {
            Writer.Write( FileUtils.Reverse( entry.Name.Value ));
            Writer.Write( Convert.ToInt32(entry.Accessory.Value) );
            WriteOffsetState( entry );
        }

        public void WriteOffsetState( AtchEntry entry )
        {
            var actualPos = ( int )( ( BodySize - ( StartPosition + 8 ) ) + StateWriter.BaseStream.Position );
            Writer.Write( actualPos );
            Writer.Write( entry.States.Count );

            foreach( var entryState in entry.States )
            {
                WriteState( entryState );
            }
        }

        public void WriteState( AtchEntryState state )
        {
            StateWriter.Write( $"{state.Bone.Value}" );
            StateWriter.Write( state.Scale.Value );
            WriteVector3( state.Offset.Value );
            WriteVector3( state.Rotation.Value );
        }

        public void WriteVector3( Vector3 input ) {
            StateWriter.Write( 3 );
            StateWriter.Write( input.X );
            StateWriter.Write( input.Y );
            StateWriter.Write( input.Z );
        }

        public override void WriteTo( BinaryWriter writer )
        {
            base.WriteTo( writer );
            writer.Write( StateMs.ToArray() );
        }
    }
}
