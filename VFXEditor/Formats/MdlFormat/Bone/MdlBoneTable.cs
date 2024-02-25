using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MdlFormat.Bone {
    public class MdlBoneTable : IUiItem {
        public readonly List<ParsedString> Bones = new();
        public readonly CommandListView<ParsedString> BoneView;

        public HashSet<string> BoneStrings => Bones.Select( x => x.Value ).ToHashSet();

        public MdlBoneTable() {
            BoneView = new( Bones, () => new( "##Name" ), true );
        }

        public MdlBoneTable( BinaryReader reader, List<string> boneStrings ) : this() {
            var boneIndexes = new List<ushort>();
            for( var i = 0; i < 64; i++ ) boneIndexes.Add( reader.ReadUInt16() );

            var boneCount = reader.ReadByte();
            reader.ReadBytes( 3 ); // padding

            for( var i = 0; i < boneCount; i++ ) {
                var bone = new ParsedString( "##Name" ) {
                    Value = boneStrings[boneIndexes[i]]
                };
                Bones.Add( bone );
            }

            BoneView = new( Bones, () => new( "##Name" ), true );
        }

        public void Draw() {
            BoneView.Draw();
        }

        public bool BonesEqual( MdlBoneTable other ) => BoneStrings.SetEquals( other.BoneStrings );

        public void PopulateWrite( MdlWriteData data ) {
            foreach( var item in Bones ) data.AddBone( item.Value );
        }

        public void Write( BinaryWriter writer, MdlWriteData data ) {
            for( var i = 0; i < 64; i++ ) {
                if( i >= Bones.Count ) {
                    writer.Write( ( ushort )0 );
                    continue;
                }

                writer.Write( ( ushort )data.BoneStrings.IndexOf( Bones[i].Value ) );
            }

            writer.Write( ( byte )Bones.Count );
            FileUtils.Pad( writer, 3 ); // padding
        }
    }
}
