using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Bone {
    public class MdlBoneTable : IUiItem {
        public readonly List<ParsedString> Bones = [];
        public readonly CommandListView<ParsedString> BoneView;

        public MdlBoneTable() {
            BoneView = new( Bones, () => new( "##Name" ), true );
        }

        public MdlBoneTable( BinaryReader reader, List<string> boneStrings ) : this() {
            var boneIndexes = new List<ushort>();

            for( var i = 0; i < 64; i++ ) {
                boneIndexes.Add( reader.ReadUInt16() );
            }

            var boneCount = reader.ReadByte();
            reader.ReadBytes( 3 ); // padding

            for( var i = 0; i < boneCount; i++ ) {
                var bone = new ParsedString( "##Name" ) {
                    Value = boneStrings[i]
                };
                Bones.Add( bone );
            }

            BoneView = new( Bones, () => new( "##Name" ), true );
        }

        public void Draw() {
            BoneView.Draw();
        }

        public void PopulateWrite( MdlWriteData data ) {
            if( Bones.Count == 0 ) return;
            data.BoneTables.Add( this );
            foreach( var item in Bones ) data.AddBone( item.Value );
        }
    }
}
