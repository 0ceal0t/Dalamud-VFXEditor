using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Utils;

namespace VfxEditor.Formats.MdlFormat.Mesh.Shape {
    public class MdlShape {
        private readonly ushort[] _ShapeMeshIndexes;
        private readonly ushort[] _ShapeMeshCounts;

        public readonly string Name;
        public readonly List<List<MdlShapeMesh>> ShapeMeshes = []; // per LoD

        // TODO: drawing

        public MdlShape( BinaryReader reader, Dictionary<uint, string> strings ) {
            Name = strings[reader.ReadUInt32()];

            _ShapeMeshIndexes = [
                reader.ReadUInt16(),
                reader.ReadUInt16(),
                reader.ReadUInt16(),
            ];

            _ShapeMeshCounts = [
                reader.ReadUInt16(),
                reader.ReadUInt16(),
                reader.ReadUInt16(),
            ];
        }

        public void Populate( MdlFileData data ) {
            for( var i = 0; i < 3; i++ ) {
                var meshes = new List<MdlShapeMesh>();
                meshes.AddRange( data.ShapesMeshes.GetRange( _ShapeMeshIndexes[i], _ShapeMeshCounts[i] ) );
                foreach( var mesh in meshes ) mesh.Populate( data );
                ShapeMeshes.Add( meshes );
            }
        }

        public void PopulateWrite( MdlWriteData data ) {
            data.AddShape( Name );
        }
    }
}
