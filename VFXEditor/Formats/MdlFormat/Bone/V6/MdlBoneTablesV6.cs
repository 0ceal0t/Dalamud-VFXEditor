using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MdlFormat.Bone.V6 {
    public class MdlBoneTablesV6 : MdlBoneTables {
        public MdlBoneTablesV6( BinaryReader reader, int count, MdlFileData data ) : base() {
            var unknowns = new List<ushort>();
            var counts = new List<ushort>();

            for( var i = 0; i < count; i++ ) {
                unknowns.Add( reader.ReadUInt16() );
                counts.Add( reader.ReadUInt16() );
            }

            for( var i = 0; i < count; i++ ) {
                Tables.Add( new MdlBoneTableV6( reader, data.BoneStrings, counts[i], unknowns[i] ) );
                if( i < count - 1 ) FileUtils.PadTo( reader, 4 );
            }
        }

        public override void Write( BinaryWriter writer, MdlWriteData data ) {
            foreach( var table in Tables ) {
                if( table is MdlBoneTableV6 v6 ) {
                    writer.Write( ( ushort )v6.Unknown.Value );
                    writer.Write( ( ushort )v6.Bones.Count );
                }
            }

            for( var i = 0; i < Tables.Count; i++ ) {
                Tables[i].Write( writer, data );
                if( i < Tables.Count - 1 ) FileUtils.PadTo( writer, 4 );
            }
        }
    }
}
