using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Utils;

namespace VfxEditor.Formats.MdlFormat.Mesh.Shape {
    public class MdlShapeMesh {
        public readonly uint _MeshIndexOffset;

        private readonly uint _ShapeValueCount;
        private readonly uint _ShapeValueOffset;

        // TODO: name
        private readonly List<MdlShapeValue> Values = [];

        public MdlShapeMesh( BinaryReader reader ) {
            _MeshIndexOffset = reader.ReadUInt32();
            _ShapeValueCount = reader.ReadUInt32();
            _ShapeValueOffset = reader.ReadUInt32();
        }

        public void Populate( MdlReaderData data ) {
            Values.AddRange( data.ShapeValues.GetRange( ( int )_ShapeValueOffset, ( int )_ShapeValueCount ) );
        }
    }
}
