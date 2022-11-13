using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitVertexes : AvfxBase {
        public readonly List<AvfxEmiterVertex> EmitVertexes = new();

        public AvfxEmitVertexes() : base( "VEmt" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 28; i++ ) EmitVertexes.Add( new AvfxEmiterVertex( reader ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var vert in EmitVertexes ) vert.Write( writer );
        }
    }

    public class AvfxEmiterVertex {
        public float[] Position = new float[3];
        public float[] Normal = new float[3];
        public int C = 0;

        public AvfxEmiterVertex() { }

        public AvfxEmiterVertex( BinaryReader reader ) {
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
