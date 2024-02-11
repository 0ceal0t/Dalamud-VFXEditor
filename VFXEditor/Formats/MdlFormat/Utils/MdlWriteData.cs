using System.Collections.Generic;

namespace VfxEditor.Formats.MdlFormat.Utils {
    public class MdlWriteData : MdlFileData {
        private uint TotalStringLength = 0;
        public readonly List<string> AllStrings = [];
        public readonly Dictionary<string, uint> StringToOffset = [];
        public readonly List<string> ShapeStrings = [];

        public void PopulateWrite( MdlFile file ) {
            foreach( var item in file.Lods ) item.PopulateWrite( this );
            // TODO: get vertex and index buffers
            // TODO: extra LoD
            // TODO: shapes
            // TODO: attributes
            foreach( var item in file.Eids ) item.PopulateWrite( this );

            // ======= GENERATE STRING OFFSETS ==========
            AddStringOffsets( AttributeStrings );
            AddStringOffsets( BoneStrings );
            AddStringOffsets( MaterialStrings );
            AddStringOffsets( ShapeStrings );
        }

        // ========= STRINGS =================

        public void AddBone( string item ) => AddString( BoneStrings, item );
        public void AddAttribute( string item ) => AddString( AttributeStrings, item );
        public void AddMaterial( string item ) => AddString( MaterialStrings, item );
        public void AddShape( string item ) => AddString( ShapeStrings, item );
        private void AddString( List<string> list, string item ) {
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
