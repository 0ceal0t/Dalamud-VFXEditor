using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Lod;
using VfxEditor.Formats.MdlFormat.Mesh;
using VfxEditor.Formats.MdlFormat.Mesh.Shape;
using VfxEditor.Formats.MdlFormat.Mesh.TerrainShadow;

namespace VfxEditor.Formats.MdlFormat.Utils {
    public class MdlWriteData : MdlFileData {
        public uint TotalStringLength { get; private set; } = 0;
        public readonly List<string> AllStrings = [];
        public readonly Dictionary<string, uint> StringToOffset = [];
        public readonly List<string> ShapeStrings = [];

        public readonly Dictionary<MdlLod, long> LodPlaceholders = [];

        public readonly List<MemoryStream> VertexData = [];
        public readonly List<MemoryStream> IndexData = [];
        private readonly List<BinaryWriter> VertexWriters = [];
        private readonly List<BinaryWriter> IndexWriters = [];

        public readonly Dictionary<MdlMesh, (uint[], uint)> MeshOffsets = [];
        public readonly Dictionary<MdlTerrainShadowMesh, (uint, uint)> TerrainShadowOffsets = [];

        public readonly List<List<MdlShapeMesh>> ShapeMeshesPerLod = [[], [], []];
        public readonly List<List<MdlShapeValue>> ShapeValuesPerLod = [[], [], []];

        public MdlWriteData( MdlFile file ) {
            for( var j = 0; j < 3; j++ ) {
                var vMs = new MemoryStream();
                var vWriter = new BinaryWriter( vMs );

                var iMs = new MemoryStream();
                var iWriter = new BinaryWriter( iMs );

                VertexData.Add( vMs );
                IndexData.Add( iMs );
                VertexWriters.Add( vWriter );
                IndexWriters.Add( iWriter );
            }

            foreach( var item in file.BoneTables ) item.PopulateWrite( this );
            for( var i = 0; i < file.AllLods.Count; i++ ) file.AllLods[i].PopulateWrite( this, i );
            for( var i = 0; i < file.ExtraLods.Count; i++ ) file.ExtraLods[i].PopulateWrite( this, i );
            foreach( var item in file.Eids ) item.PopulateWrite( this );
            foreach( var item in file.Shapes ) item.PopulateWrite( this );

            for( var i = 0; i < 3; i++ ) {
                ShapesMeshes.AddRange( ShapeMeshesPerLod[i] );
                ShapeValues.AddRange( ShapeValuesPerLod[i] );
            }

            // ======= GENERATE STRING OFFSETS ==========

            AddStringOffsets( AttributeStrings );
            AddStringOffsets( BoneStrings );
            AddStringOffsets( MaterialStrings );
            AddStringOffsets( ShapeStrings );
        }

        public void Dispose() {
            foreach( var item in VertexWriters ) item.Dispose();
            foreach( var item in IndexWriters ) item.Dispose();
            foreach( var item in VertexData ) item.Dispose();
            foreach( var item in IndexData ) item.Dispose();
        }

        // ========= VERTEX + INDEX DATA ===============

        public void AddVertexData( MdlMesh mesh, List<byte[]> vertexData, byte[] indexData, int lod ) {
            var vWriter = VertexWriters[lod];
            var iWriter = IndexWriters[lod];

            var iOffset = ( uint )IndexData[lod].Position / 2;
            iWriter.Write( indexData );

            var vOffsets = new uint[] { 0, 0, 0 };
            for( var i = 0; i < vertexData.Count; i++ ) {
                vOffsets[i] = ( uint )VertexData[lod].Position;
                vWriter.Write( vertexData[i] );
            }

            MeshOffsets[mesh] = (vOffsets, iOffset);
        }

        public void AddVertexData( MdlTerrainShadowMesh mesh, byte[] vertexData, byte[] indexData, int lod ) {
            var vWriter = VertexWriters[lod];
            var iWriter = IndexWriters[lod];

            var iOffset = ( uint )IndexData[lod].Position / 2;
            iWriter.Write( indexData );

            var vOffset = ( uint )VertexData[lod].Position;
            vWriter.Write( vertexData );

            TerrainShadowOffsets[mesh] = (vOffset, iOffset);
        }

        // ========= MESH OFFSETS =================

        public void WriteIndexCount( BinaryWriter writer, List<MdlMesh> items, int defaultOffset ) => WriteIndexCount( writer, Meshes, items, defaultOffset );

        public void WriteIndexCount( BinaryWriter writer, List<MdlTerrainShadowMesh> items, int defaultOffset ) => WriteIndexCount( writer, TerrainShadowMeshes, items, defaultOffset );

        public void WriteIndexCount( BinaryWriter writer, List<MdlSubMesh> items, int defaultOffset ) => WriteIndexCount( writer, SubMeshes, items, defaultOffset );

        public void WriteIndexCount( BinaryWriter writer, List<MdlTerrainShadowSubmesh> items, int defaultOffset ) => WriteIndexCount( writer, TerrainShadowSubmeshes, items, defaultOffset );

        private static void WriteIndexCount<T>( BinaryWriter writer, List<T> allItems, List<T> items, int defaultOffset ) {
            var offset = items.Count == 0 ? defaultOffset : allItems.IndexOf( items[0] );
            writer.Write( ( ushort )offset );
            writer.Write( ( ushort )items.Count );
        }

        // ========= STRINGS =================

        public void AddBone( string item ) => AddString( BoneStrings, item );
        public void AddAttribute( string item ) => AddString( AttributeStrings, item );
        public void AddMaterial( string item ) => AddString( MaterialStrings, item );
        public void AddShape( string item ) => AddString( ShapeStrings, item );

        private static void AddString( List<string> list, string item ) {
            if( !list.Contains( item ) ) list.Add( item );
        }

        private void AddStringOffsets( List<string> list ) {
            foreach( var item in list ) {
                AllStrings.Add( item );
                OffsetToString[TotalStringLength] = item;
                StringToOffset[item] = TotalStringLength;
                TotalStringLength += ( uint )item.Length + 1; // Null at the end
            }
        }
    }
}
