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

        public readonly Dictionary<MdlLod, long> LodOffsets = [];

        private int MeshIndex = 0;
        private int SubmeshIndex = 0;

        private int TerrainShadowMeshIndex = 0;
        private int TerrainShadowSubmeshIndex = 0;

        public MdlWriteData( MdlFile file ) {
            for( var i = 0; i < file.Lods.Count; i++ ) file.Lods[i].PopulateWrite( this, i );
            for( var i = 0; i < file.ExtraLods.Count; i++ ) file.ExtraLods[i].PopulateWrite( this, i );
            // TODO: get vertex and index buffers
            foreach( var item in file.Eids ) item.PopulateWrite( this );
            foreach( var item in file.Shapes ) item.PopulateWrite( this );

            // ======= GENERATE STRING OFFSETS ==========
            AddStringOffsets( AttributeStrings );
            AddStringOffsets( BoneStrings );
            AddStringOffsets( MaterialStrings );
            AddStringOffsets( ShapeStrings );
        }

        // ========= MESHES =================

        public void WriteIndexCount( BinaryWriter writer, List<MdlMesh> items ) => WriteIndexCount( writer, Meshes, items, ref MeshIndex );

        public void WriteIndexCount( BinaryWriter writer, List<MdlTerrainShadowMesh> items ) => WriteIndexCount( writer, TerrainShadowMeshes, items, ref TerrainShadowMeshIndex );

        public void WriteIndexCount( BinaryWriter writer, List<MdlSubMesh> items ) => WriteIndexCount( writer, SubMeshes, items, ref SubmeshIndex );

        public void WriteIndexCount( BinaryWriter writer, List<MdlTerrainShadowSubmesh> items ) => WriteIndexCount( writer, TerrainShadowSubmeshes, items, ref TerrainShadowSubmeshIndex );

        private static void WriteIndexCount<T>( BinaryWriter writer, List<T> allItems, List<T> items, ref int index ) {
            var offset = items.Count == 0 ? index : allItems.IndexOf( items[0] );
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
