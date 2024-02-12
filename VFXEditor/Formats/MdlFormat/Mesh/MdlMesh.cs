using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.Formats.MdlFormat.Bone;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Formats.MdlFormat.Vertex;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.MdlFormat.Mesh {
    public struct DataRange {
        public uint Start;
        public uint End;
        public uint Count;
        public uint Stride;
    }

    public class MdlMesh : MdlMeshData {
        public readonly MdlVertexDeclaration Format;

        private readonly ushort _MaterialStringIdx;
        private readonly ushort _SubmeshIndex;
        private readonly ushort _SubmeshCount;
        private readonly uint[] _VertexBufferOffsets;
        private readonly ushort _BoneTableIndex;

        private readonly ParsedString Material = new( "Material" );

        private MdlBoneTable BoneTable;

        private readonly ushort VertexCount; // Maxes out at ushort.MaxValue

        private List<byte[]> RawVertexData = [[], [], []];

        private readonly List<MdlSubMesh> Submeshes = [];
        private readonly UiSplitView<MdlSubMesh> SubmeshView;

        public MdlMesh( MdlFile file, MdlVertexDeclaration format, BinaryReader reader ) : base( file ) {
            Format = format;

            VertexCount = reader.ReadUInt16();
            reader.ReadUInt16(); // padding
            IndexCount = reader.ReadUInt32();
            _MaterialStringIdx = reader.ReadUInt16();
            _SubmeshIndex = reader.ReadUInt16();
            _SubmeshCount = reader.ReadUInt16();
            _BoneTableIndex = reader.ReadUInt16();
            _IndexOffset = 2 * reader.ReadUInt32();

            _VertexBufferOffsets = [reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32()];
            reader.ReadBytes( 3 ); // strides
            reader.ReadByte(); // stream count

            SubmeshView = new( "Sub-Mesh", Submeshes, false );
        }

        public override Vector4[] GetData( int indexCount, byte[] rawIndexData ) => Format.GetData( rawIndexData, RawVertexData, indexCount, VertexCount );

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Preview" ) ) {
                if( tab ) DrawMesh();
            }

            using( var tab = ImRaii.TabItem( "Sub-Meshes" ) ) {
                if( tab ) SubmeshView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Bone Table" ) ) {
                if( tab ) BoneTable.Draw();
            }
        }

        private void DrawMesh() {
            Material.Draw();
            DrawPreview();
        }

        public void Populate( MdlFileData data, BinaryReader reader, int lod ) {
            Populate( reader, data.IndexBufferOffsets[lod] );

            RawVertexData = [];
            for( var i = 0; i < 3; i++ ) {
                var stride = Format.GetStride( i );
                if( stride == 0 ) continue;
                reader.BaseStream.Position = data.VertexBufferOffsets[lod] + _VertexBufferOffsets[i];
                RawVertexData.Add( reader.ReadBytes( VertexCount * stride ) );
            }

            Submeshes.AddRange( data.SubMeshes.GetRange( _SubmeshIndex, _SubmeshCount ) );
            foreach( var submesh in Submeshes ) submesh.Populate( this, data, reader, data.IndexBufferOffsets[lod] );

            Material.Value = data.MaterialStrings[_MaterialStringIdx];

            // Skip bone table if no bones
            BoneTable = _BoneTableIndex == 255 ? new() : data.BoneTables[_BoneTableIndex]; // TODO: check if empty when writing

            foreach( var shape in data.Shapes ) {
                var shapeMeshes = shape.ShapeMeshes[lod].Where( x => x._MeshIndexOffset == _IndexOffset / 2 ); // TODO
            }
        }

        public void PopulateWrite( MdlWriteData data, int lod ) {
            data.Meshes.Add( this );
            data.AddMaterial( Material.Value );
            BoneTable.PopulateWrite( data );

            foreach( var item in Submeshes ) item.PopulateWrite( data, lod );

            // TODO
        }
    }
}
