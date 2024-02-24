using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Utils;

namespace VfxEditor.Formats.MdlFormat.Mesh.Shape {
    public class MdlShapeMesh {
        private MdlShape Parent;

        public string Name => Parent?.Name;
        public readonly uint _MeshIndexOffset;

        private readonly uint _ShapeValueCount;
        private readonly uint _ShapeValueOffset;

        private readonly List<MdlShapeValue> Values = new();

        public MdlShapeMesh( BinaryReader reader ) {
            _MeshIndexOffset = reader.ReadUInt32();
            _ShapeValueCount = reader.ReadUInt32();
            _ShapeValueOffset = reader.ReadUInt32();
        }

        public void Populate( MdlShape parent, MdlFileData data ) {
            Parent = parent;
            Values.AddRange( data.ShapeValues.GetRange( ( int )_ShapeValueOffset, ( int )_ShapeValueCount ) );
        }

        public void PopulateWrite( MdlWriteData data, int lod ) {
            data.ShapeMeshesPerLod[lod].Add( this );

            foreach( var item in Values ) item.PopulateWrite( data, lod );
        }

        public void Write( BinaryWriter writer, MdlWriteData data ) {
            writer.Write( _MeshIndexOffset );

            // uint for some reason
            writer.Write( Values.Count );
            writer.Write( Values.Count == 0 ? 0 : data.ShapeValues.IndexOf( Values[0] ) );
        }
    }
}
