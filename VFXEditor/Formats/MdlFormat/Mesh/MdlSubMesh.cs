using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.Formats.MdlFormat.Mesh {
    public class MdlSubMesh : MdlSubmeshData<MdlMesh> {
        private readonly uint _AttributeIndexMask;
        private readonly ushort _BoneStartIndex;
        private readonly ushort _BoneCount;

        public readonly List<ParsedString> Bones = [];
        public readonly CommandListView<ParsedString> BoneView;

        public readonly List<ParsedString> Attributes = [];
        public readonly CommandListView<ParsedString> AttributeView;

        public MdlSubMesh( BinaryReader reader ) : base( null ) {
            _IndexOffset = 2 * reader.ReadUInt32();
            IndexCount = reader.ReadUInt32();
            _AttributeIndexMask = reader.ReadUInt32();
            _BoneStartIndex = reader.ReadUInt16();
            _BoneCount = reader.ReadUInt16();

            BoneView = new( Bones, () => new( "##Bone" ), true );
            AttributeView = new( Attributes, () => new( "##Attribute" ), true );
        }

        public void Populate( MdlMesh parent, MdlFileData data, BinaryReader reader, uint indexBufferPos ) {
            Populate( parent, reader, indexBufferPos );

            for( var i = 0; i < _BoneCount; i++ ) {
                var bone = new ParsedString( "##Bone" ) {
                    Value = ( ( _BoneStartIndex + i ) < data.SubmeshBoneMap.Count ) ?
                        ( data.SubmeshBoneMap[_BoneStartIndex + i] < data.StringTable.BoneStrings.Count ?
                            data.StringTable.BoneStrings[data.SubmeshBoneMap[_BoneStartIndex + i]] : "[ERROR]" ) : "[ERROR]"
                };
                Bones.Add( bone );
            }

            for( var i = 0; i < data.StringTable.AttributeStrings.Count; i++ ) {
                if( ( _AttributeIndexMask & ( 1u << i ) ) != 0 ) {
                    var attr = new ParsedString( "##Atribute" ) {
                        Value = ( i < data.StringTable.AttributeStrings.Count ) ? data.StringTable.AttributeStrings[i] : "[ERROR]"
                    };
                    Attributes.Add( attr );
                }
            }
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Preview" ) ) {
                if( tab ) DrawPreview();
            }

            using( var tab = ImRaii.TabItem( "Bones" ) ) {
                if( tab ) BoneView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Attributes" ) ) {
                if( tab ) AttributeView.Draw();
            }
        }

        public void PopulateWrite( MdlWriteData data ) {
            data.SubMeshes.Add( this );
            foreach( var item in Bones ) data.AddBone( item.Value );
            foreach( var item in Attributes ) data.AddAttribute( item.Value );
        }

        public void Write( BinaryWriter writer, MdlWriteData data ) {
            writer.Write( _IndexOffset / 2 );
            writer.Write( IndexCount );

            var attributeMask = _AttributeIndexMask;
            var selected = Attributes.Select( x => x.Value ).ToList();
            for( var i = 0; i < data.StringTable.AttributeStrings.Count; i++ ) {
                var value = 1u << i;
                if( selected.Contains( data.StringTable.AttributeStrings[i] ) ) {
                    attributeMask |= value;
                }
                else {
                    attributeMask &= ~value;
                }
            }
            writer.Write( attributeMask );

            writer.Write( ( ushort )( Bones.Count == 0 ? 0xFFFF : data.SubmeshBoneMap.Count ) );
            writer.Write( ( ushort )Bones.Count );

            foreach( var bone in Bones ) {
                data.SubmeshBoneMap.Add( ( ushort )data.StringTable.BoneStrings.IndexOf( bone.Value ) );
            }
        }
    }
}
