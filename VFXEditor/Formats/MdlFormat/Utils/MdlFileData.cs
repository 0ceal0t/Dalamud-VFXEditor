using System.Collections.Generic;
using System.Linq;
using VfxEditor.Formats.MdlFormat.Mesh;
using VfxEditor.Formats.MdlFormat.Mesh.Shape;
using VfxEditor.Formats.MdlFormat.Mesh.TerrainShadow;

namespace VfxEditor.Formats.MdlFormat.Utils {
    public class MdlFileData {
        public readonly List<uint> VertexBufferSizes = [];
        public readonly List<uint> IndexBufferSizes = [];
        public readonly List<uint> VertexBufferOffsets = [];
        public readonly List<uint> IndexBufferOffsets = [];

        public readonly List<MdlMesh> Meshes = [];
        public readonly List<MdlTerrainShadowMesh> TerrainShadowMeshes = [];
        public readonly List<MdlSubMesh> SubMeshes = [];
        public readonly List<MdlTerrainShadowSubmesh> TerrainShadowSubmeshes = [];

        public readonly Dictionary<uint, string> OffsetToString = [];
        public readonly List<string> AttributeStrings = [];
        public readonly List<string> MaterialStrings = [];
        public readonly List<string> BoneStrings = [];

        public readonly List<ushort> SubmeshBoneMap = [];

        public readonly List<MdlShape> Shapes = [];
        public readonly List<MdlShapeMesh> ShapesMeshes = [];
        public readonly List<MdlShapeValue> ShapeValues = [];

        public int GetIndexPadding( int lod ) {
            var expected = ( int )IndexBufferSizes[lod];
            var actual =
                Meshes.Where( x => x.Lod == lod ).Select( x => x.RawIndexDataSize ).Sum() +
                TerrainShadowMeshes.Where( x => x.Lod == lod ).Select( x => x.RawIndexDataSize ).Sum();
            return expected - actual;
        }
    }
}
