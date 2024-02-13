using System.IO;
using VfxEditor.Formats.MdlFormat.Utils;

namespace VfxEditor.Formats.MdlFormat.Mesh.Shape {
    public class MdlShapeValue {
        private readonly ushort _BaseIndicesIndex; // Index into the Indices array of a mesh
        private readonly ushort _ReplacingVertexIndex; // Index into the (without transformation probably unused) vertex of a mesh

        public MdlShapeValue( BinaryReader reader ) {
            _BaseIndicesIndex = reader.ReadUInt16();
            _ReplacingVertexIndex = reader.ReadUInt16();
        }

        public void PopulateWrite( MdlWriteData data ) {
            data.ShapeValues.Add( this );
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( _BaseIndicesIndex );
            writer.Write( _ReplacingVertexIndex );
        }
    }
}
