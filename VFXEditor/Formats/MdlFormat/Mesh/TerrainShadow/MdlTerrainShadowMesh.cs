using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.MdlFormat.Mesh.TerrainShadow {
    public class MdlTerrainShadowMesh : MdlMeshData {
        private readonly ushort _SubmeshIndex;
        private readonly ushort _SubmeshCount;
        private readonly uint _VertexBufferOffset;

        private ushort VertexCount;

        private byte[] RawVertexData = Array.Empty<byte>();

        private readonly List<MdlTerrainShadowSubmesh> Submeshes = new();
        private readonly CommandSplitView<MdlTerrainShadowSubmesh> SubmeshView;

        public MdlTerrainShadowMesh( MdlFile file ) : base( file ) {
            SubmeshView = new( "Sub-Mesh", Submeshes, false, null, () => new( this ) );
        }

        public MdlTerrainShadowMesh( MdlFile file, BinaryReader reader ) : this( file ) {
            IndexCount = reader.ReadUInt32();
            _IndexOffset = 2 * reader.ReadUInt32();
            _VertexBufferOffset = reader.ReadUInt32();
            VertexCount = reader.ReadUInt16();
            _SubmeshIndex = reader.ReadUInt16();
            _SubmeshCount = reader.ReadUInt16();
            var stride = reader.ReadByte();
            reader.ReadByte(); // padding

            if( stride != 8 ) {
                Dalamud.Log( $"Terrain Shadow: stride={stride}" );
            }
        }

        public override Vector4[] GetData( int indexCount, byte[] rawIndexData ) {
            var data = new List<Vector4>();

            var positions = new List<Vector4>();

            using var ms = new MemoryStream( RawVertexData );
            using var reader = new BinaryReader( ms );

            for( var i = 0; i < VertexCount; i++ ) {
                positions.Add( new( ( float )reader.ReadHalf(), ( float )reader.ReadHalf(), ( float )reader.ReadHalf(), ( float )reader.ReadHalf() ) );
            }

            using var iMs = new MemoryStream( rawIndexData );
            using var iReader = new BinaryReader( iMs );

            for( var i = 0; i < indexCount; i++ ) {
                var index = iReader.ReadInt16();

                data.Add( positions[index] );
                data.Add( new( 1, 0, 0, 1 ) ); // tangent
                data.Add( new( 0, 0, 0, 0 ) ); // uv
                data.Add( new( 0, 1, 0, 1 ) ); // normal
                data.Add( new( 1, 1, 1, 1 ) ); // colot
            }

            return data.ToArray();
        }

        public void Populate( MdlReaderData data, BinaryReader reader, int lod ) {

            Populate( reader, data.IndexBufferOffsets[lod] );

            reader.BaseStream.Position = data.VertexBufferOffsets[lod] + _VertexBufferOffset;
            RawVertexData = reader.ReadBytes( VertexCount * 8 );

            Submeshes.AddRange( data.TerrainShadowSubmeshes.GetRange( _SubmeshIndex, _SubmeshCount ) );
            foreach( var submesh in Submeshes ) submesh.Populate( this, reader, data.IndexBufferOffsets[lod] );
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Mesh" ) ) {
                if( tab ) DrawPreview();
            }

            using( var tab = ImRaii.TabItem( "Sub-Meshes" ) ) {
                if( tab ) SubmeshView.Draw();
            }
        }
    }
}
