using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MdlFormat.Bone.V6 {
    public class MdlBoneTablesV6 : MdlBoneTables {
        public MdlBoneTablesV6( BinaryReader reader, int count, MdlFileData data ) : base() {
            var header = new List<(ushort, ushort)>(); // offset, count

            for( var i = 0; i < count; i++ ) header.Add( (reader.ReadUInt16(), reader.ReadUInt16()) );

            for( var i = 0; i < count; i++ ) {
                var (_, tableCount) = header[i];
                Tables.Add( new MdlBoneTableV6( reader, data.BoneStrings, tableCount ) );
                if( i < count - 1 ) FileUtils.PadTo( reader, 4 );
            }
        }

        public override void Write( BinaryWriter writer, MdlWriteData data ) {
            var startPos = writer.BaseStream.Position;
            foreach( var _ in Tables ) {
                writer.Write( ( ushort )0 ); // temp
                writer.Write( ( ushort )0 ); // temp
            }

            var positions = new List<long>();
            for( var i = 0; i < Tables.Count; i++ ) {
                positions.Add( writer.BaseStream.Position );
                Tables[i].Write( writer, data );
                if( i < Tables.Count - 1 ) FileUtils.PadTo( writer, 4 );
            }
            var endPos = writer.BaseStream.Position;

            writer.BaseStream.Position = startPos;
            for( var i = 0; i < Tables.Count; i++ ) {
                writer.Write( ( ushort )( ( positions[i] - writer.BaseStream.Position ) / 4 ) );
                writer.Write( ( ushort )Tables[i].Bones.Count );
            }
            writer.BaseStream.Position = endPos;
        }
    }
}
