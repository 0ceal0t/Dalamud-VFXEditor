using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Vertex;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Mesh {
    public struct DataRange {
        public uint Start;
        public uint End;
        public uint Count;
        public uint Stride;
    }

    public class MdlMesh : MdlMeshDrawable, IUiItem {
        public readonly MdlFile File;
        public readonly MdlVertexDeclaration Format;

        private ushort _MaterialStringIdx;
        private readonly ushort _SubmeshIndex;
        private readonly ushort _SubmeshCount;
        private readonly uint _IndexOffset;
        private readonly uint[] _VertexBufferOffsets;

        private readonly ParsedString Material = new( "Material" );
        private readonly ParsedShort BoneTableIndex = new( "Bone Table Index" );

        private ushort VertexCount; // Maxes out at ushort.MaxValue
        private uint IndexCount;

        private byte[] RawIndexData;
        private List<byte[]> RawVertexData;

        private readonly List<MdlSubMesh> Submeshes = new();

        // TODO: creating new
        public MdlMesh( MdlFile file ) {
            Format = new();
            File = file;
        }

        public MdlMesh( MdlFile file, MdlVertexDeclaration format, BinaryReader reader ) : this( file ) {
            Format = format;

            VertexCount = reader.ReadUInt16();
            reader.ReadUInt16(); // padding
            IndexCount = reader.ReadUInt32();
            _MaterialStringIdx = reader.ReadUInt16();
            _SubmeshIndex = reader.ReadUInt16();
            _SubmeshCount = reader.ReadUInt16();
            BoneTableIndex.Read( reader );
            _IndexOffset = 2 * reader.ReadUInt32();

            _VertexBufferOffsets = new[] { reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32() };
            reader.ReadBytes( 3 ); // strides
            reader.ReadByte(); // stream count
        }

        public Buffer GetBuffer( Device device, out int vertexCount ) {
            if( Data == null ) RefreshBuffer( device );
            vertexCount = VertexCount;
            return Data;
        }

        public override ushort GetVertexCount() => VertexCount;

        public override void RefreshBuffer( Device device ) {
            Data?.Dispose();
            var data = Format.GetData( RawIndexData, RawVertexData, ( int )IndexCount, VertexCount );
            Data = Buffer.Create( device, BindFlags.VertexBuffer, data );
        }

        public void Populate( BinaryReader reader, uint vertexBufferPos, uint indexBufferPos ) {
            reader.BaseStream.Position = indexBufferPos + _IndexOffset;
            RawIndexData = reader.ReadBytes( ( int )( IndexCount * 2 ) );

            RawVertexData = new();
            for( var i = 0; i < 3; i++ ) {
                var stride = Format.GetStride( i );
                if( stride == 0 ) continue;
                reader.BaseStream.Position = vertexBufferPos + _VertexBufferOffsets[i];
                RawVertexData.Add( reader.ReadBytes( VertexCount * stride ) );
            }
        }

        public void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Mesh" ) ) {
                if( tab ) DrawMesh();
            }

            using( var tab = ImRaii.TabItem( "Sub-Meshes" ) ) {
                //if( tab ) SubmeshView.Draw();
            }
        }

        private void DrawMesh() {
            Material.Draw();
            BoneTableIndex.Draw();

            if( Plugin.DirectXManager.MeshPreview.CurrentMesh != this ) {
                Plugin.DirectXManager.MeshPreview.LoadMesh( File, this );
            }
            Plugin.DirectXManager.MeshPreview.DrawInline();
        }
    }
}
