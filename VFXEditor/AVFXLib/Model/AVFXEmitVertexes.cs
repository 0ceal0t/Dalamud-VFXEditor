using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Model {
    public class AVFXEmitVertexes : AVFXBase {
        public readonly List<AVFXEmitVertex> EmitVertexes = new();

        public AVFXEmitVertexes() : base( "VEmt" ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var count = size / 28;
            for( var i = 0; i < count; i++ ) {
                EmitVertexes.Add( new AVFXEmitVertex( reader ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var vert in EmitVertexes ) vert.Write( writer );
        }
    }

    public class AVFXEmitVertex {
        public float[] Position = new float[3];
        public float[] Normal = new float[3];
        public int C = 0;

        public AVFXEmitVertex() {

        }

        public AVFXEmitVertex( BinaryReader reader ) {
            Position[0] = reader.ReadSingle();
            Position[1] = reader.ReadSingle();
            Position[2] = reader.ReadSingle();

            Normal[0] = reader.ReadSingle();
            Normal[1] = reader.ReadSingle();
            Normal[2] = reader.ReadSingle();

            C = reader.ReadInt32();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Position[0] );
            writer.Write( Position[1] );
            writer.Write( Position[2] );

            writer.Write( Normal[0] );
            writer.Write( Normal[1] );
            writer.Write( Normal[2] );

            writer.Write( C );
        }
    }
}
