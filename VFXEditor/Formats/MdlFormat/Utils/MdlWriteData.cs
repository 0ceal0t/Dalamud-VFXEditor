using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Lod;
using VfxEditor.Formats.MdlFormat.Mesh;
using VfxEditor.Formats.MdlFormat.Mesh.TerrainShadow;

namespace VfxEditor.Formats.MdlFormat.Utils {
    public class MdlWriteData : MdlFileData {
        public uint TotalStringLength { get; private set; } = 0;
        public readonly List<string> AllStrings = [];
        public readonly Dictionary<string, uint> StringToOffset = [];
        public readonly List<string> ShapeStrings = [];

        public readonly Dictionary<MdlLod, long> LodPlaceholders = [];

        private int MeshIndex = 0;
        private int SubmeshIndex = 0;

        private int TerrainShadowMeshIndex = 0;
        private int TerrainShadowSubmeshIndex = 0;

        private readonly List<MemoryStream> VertexData = [];
        private readonly List<MemoryStream> IndexData = [];
        private readonly List<BinaryWriter> VertexWriters = [];
        private readonly List<BinaryWriter> IndexWriters = [];

        public readonly Dictionary<MdlMesh, (uint[], uint)> MeshOffsets = [];
        public readonly Dictionary<MdlTerrainShadowMesh, (uint, uint)> TerrainShadowOffsets = [];

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

            for( var i = 0; i < file.AllLods.Count; i++ ) file.AllLods[i].PopulateWrite( this, i );
            for( var i = 0; i < file.ExtraLods.Count; i++ ) file.ExtraLods[i].PopulateWrite( this, i );
            foreach( var item in file.Eids ) item.PopulateWrite( this );
            foreach( var item in file.Shapes ) item.PopulateWrite( this );

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

        public void WriteIndexCount( BinaryWriter writer, List<MdlMesh> items, bool useIndex = true ) => WriteIndexCount( writer, Meshes, items, useIndex, ref MeshIndex );

        public void WriteIndexCount( BinaryWriter writer, List<MdlTerrainShadowMesh> items, bool useIndex = true ) => WriteIndexCount( writer, TerrainShadowMeshes, items, useIndex, ref TerrainShadowMeshIndex );

        public void WriteIndexCount( BinaryWriter writer, List<MdlSubMesh> items, bool useIndex = true ) => WriteIndexCount( writer, SubMeshes, items, useIndex, ref SubmeshIndex );

        public void WriteIndexCount( BinaryWriter writer, List<MdlTerrainShadowSubmesh> items, bool useIndex = true ) => WriteIndexCount( writer, TerrainShadowSubmeshes, items, useIndex, ref TerrainShadowSubmeshIndex );

        private static void WriteIndexCount<T>( BinaryWriter writer, List<T> allItems, List<T> items, bool useIndex, ref int index ) {
            var offset = items.Count == 0 ? ( useIndex ? index : 0 ) : allItems.IndexOf( items[0] );
            writer.Write( ( ushort )offset );
            writer.Write( ( ushort )items.Count );
            index += items.Count;
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
