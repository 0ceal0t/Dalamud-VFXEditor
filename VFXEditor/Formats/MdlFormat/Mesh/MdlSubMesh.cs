using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.Formats.MdlFormat.Mesh {
    public class MdlSubMesh : MdlSubmeshData<MdlMesh> {
        private readonly uint _AttributeIndexMask;
        private readonly ushort _BoneStartIndex;
        private readonly ushort _BoneCount;

        public readonly List<ParsedString> Bones = new();
        public readonly CommandListView<ParsedString> BoneView;

        public MdlSubMesh( MdlMesh parent ) : base( parent ) {
            BoneView = new( Bones, () => new( "##Name" ), true );
        }

        public MdlSubMesh( BinaryReader reader ) : this( ( MdlMesh )null ) {
            _IndexOffset = 2 * reader.ReadUInt32();
            IndexCount = reader.ReadUInt32();
            _AttributeIndexMask = reader.ReadUInt32();
            _BoneStartIndex = reader.ReadUInt16();
            _BoneCount = reader.ReadUInt16();
        }

        public void Populate( MdlMesh parent, BinaryReader reader, uint indexBufferPos, List<string> boneStrings ) {
            Populate( parent, reader, indexBufferPos );
            for( var i = 0; i < _BoneCount; i++ ) {
                var bone = new ParsedString( "##Name" ) {
                    Value = boneStrings[i + _BoneStartIndex]
                };
                Bones.Add( bone );
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
        }
    }
}
