using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.AVFXLib.Model {
    public class AVFXModel : AVFXBase {
        public const string NAME = "Modl";

        public readonly AVFXVNums VNums = new();
        public readonly AVFXVertexes Vertexes = new();
        public readonly AVFXIndexes Indexes = new();
        public readonly AVFXEmitVertexes EmitVertexes = new();

        private readonly List<AVFXBase> Children;

        public AVFXModel() : base( NAME ) {
            Children = new List<AVFXBase> {
                VNums,
                EmitVertexes,
                Vertexes,
                Indexes
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            if (VNums.Nums.Count > 0) VNums.Write( writer );
            if (EmitVertexes.EmitVertexes.Count > 0) EmitVertexes.Write( writer );
            if (Vertexes.Vertexes.Count > 0) Vertexes.Write( writer );
            if (Indexes.Indexes.Count > 0) Indexes.Write( writer );
        }
    }
}
