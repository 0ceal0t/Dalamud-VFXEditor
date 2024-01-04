using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.MdlFormat.Bone;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
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

        private ushort VertexCount; // Maxes out at ushort.MaxValue

        private List<byte[]> RawVertexData = new() { Array.Empty<byte>(), Array.Empty<byte>(), Array.Empty<byte>() };

        private readonly List<MdlSubMesh> Submeshes = new();
        private readonly CommandSplitView<MdlSubMesh> SubmeshView;

        public MdlMesh( MdlFile file ) : base( file ) {
            Format = new();
            SubmeshView = new( "Sub-Mesh", Submeshes, false, null, () => new( this ) );
        }

        public MdlMesh( MdlFile file, MdlVertexDeclaration format, BinaryReader reader ) : this( file ) {
            Format = format;

            VertexCount = reader.ReadUInt16();
            reader.ReadUInt16(); // padding
            IndexCount = reader.ReadUInt32();
            _MaterialStringIdx = reader.ReadUInt16();
            _SubmeshIndex = reader.ReadUInt16();
            _SubmeshCount = reader.ReadUInt16();
            _BoneTableIndex = reader.ReadUInt16();
            _IndexOffset = 2 * reader.ReadUInt32();

            _VertexBufferOffsets = new[] { reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32() };
            reader.ReadBytes( 3 ); // strides
            reader.ReadByte(); // stream count
        }

        public override Vector4[] GetData( int indexCount, byte[] rawIndexData ) => Format.GetData( rawIndexData, RawVertexData, indexCount, VertexCount );

        public void Populate(
            List<MdlSubMesh> submeshes, BinaryReader reader,
            uint vertexBufferPos, uint indexBufferPos,
            List<string> materialStrings, List<string> boneStrings,
            List<MdlBoneTable> boneTables ) {

            Populate( reader, indexBufferPos );

            RawVertexData = new();
            for( var i = 0; i < 3; i++ ) {
                var stride = Format.GetStride( i );
                if( stride == 0 ) continue;
                reader.BaseStream.Position = vertexBufferPos + _VertexBufferOffsets[i];
                RawVertexData.Add( reader.ReadBytes( VertexCount * stride ) );
            }

            Submeshes.AddRange( submeshes.GetRange( _SubmeshIndex, _SubmeshCount ) );
            foreach( var submesh in Submeshes ) submesh.Populate( this, reader, indexBufferPos, boneStrings );

            Material.Value = materialStrings[_MaterialStringIdx];

            BoneTable = boneTables[_BoneTableIndex];
        }

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
    }
}
