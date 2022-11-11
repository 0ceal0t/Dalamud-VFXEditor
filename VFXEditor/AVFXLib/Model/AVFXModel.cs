using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Model {
    public class AVFXModel : AVFXBase {
        public const string NAME = "Modl";

        public readonly AVFXVertexNumbers VertexNumbers = new();
        public readonly AVFXVertexes Vertexes = new();
        public readonly AVFXIndexes Indexes = new();
        public readonly AVFXEmitVertexes EmitVertexes = new();

        private readonly List<AVFXBase> Children;

        public AVFXModel() : base( NAME ) {
            Children = new List<AVFXBase> {
                VertexNumbers,
                EmitVertexes,
                Vertexes,
                Indexes
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            if( VertexNumbers.VertexNumbers.Count > 0 ) VertexNumbers.Write( writer );
            if( EmitVertexes.EmitVertexes.Count > 0 ) EmitVertexes.Write( writer );
            if( Vertexes.Vertexes.Count > 0 ) Vertexes.Write( writer );
            if( Indexes.Indexes.Count > 0 ) Indexes.Write( writer );
        }
    }
}
