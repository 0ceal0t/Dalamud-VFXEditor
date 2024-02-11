using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
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

        public void Populate( MdlMesh parent, MdlReaderData data, BinaryReader reader, uint indexBufferPos ) {
            Populate( parent, reader, indexBufferPos );

            for( var i = 0; i < _BoneCount; i++ ) {
                var bone = new ParsedString( "##Bone" ) {
                    Value = data.BoneStrings[data.SubmeshBoneMap[_BoneStartIndex + i]],
                };
                Bones.Add( bone );
            }

            for( var i = 0; i < data.AttributeStrings.Count; i++ ) {
                if( ( _AttributeIndexMask & ( 1u << i ) ) != 0 ) {
                    var attr = new ParsedString( "##Atribute" ) {
                        Value = data.AttributeStrings[i]
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
    }
}
