using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxModel : AvfxNode {
        public const string NAME = "Modl";

        public readonly AvfxVertexNumbers VertexNumbers = new();
        public readonly AvfxVertexes Vertexes = new();
        public readonly AvfxIndexes Indexes = new();
        public readonly AvfxEmitVertexes EmitVertexes = new();

        private readonly List<AvfxBase> Children;

        public AvfxModel() : base( NAME, UiNodeGroup.ModelColor, false ) {
            Children = new() {
                VertexNumbers,
                EmitVertexes,
                Vertexes,
                Indexes
            };
            HasDependencies = false;
        }

        public override void Draw( string parentId ) {
            // TODO
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            if( VertexNumbers.VertexNumbers.Count > 0 ) VertexNumbers.Write( writer );
            if( EmitVertexes.EmitVertexes.Count > 0 ) EmitVertexes.Write( writer );
            if( Vertexes.Vertexes.Count > 0 ) Vertexes.Write( writer );
            if( Indexes.Indexes.Count > 0 ) Indexes.Write( writer );
        }

        public override string GetDefaultText() => $"Model {GetIdx()}";

        public override string GetWorkspaceId() => $"Mdl{GetIdx()}";
    }
}
