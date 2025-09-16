using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.MdlFormat.Mesh.TerrainShadow {
    public class MdlTerrainShadowMesh : MdlMeshData {
        public int Lod { get; private set; } = -1;

        private readonly ushort _SubmeshIndex;
        private readonly ushort _SubmeshCount;
        private readonly uint _VertexBufferOffset;

        private readonly ushort VertexCount;

        private byte[] RawVertexData = [];

        private readonly List<MdlTerrainShadowSubmesh> Submeshes = [];
        private readonly UiSplitView<MdlTerrainShadowSubmesh> SubmeshView;

        public MdlTerrainShadowMesh( MdlFile file, BinaryReader reader ) : base( file ) {
            IndexCount = reader.ReadUInt32();
            _IndexOffset = 2 * reader.ReadUInt32();
            _VertexBufferOffset = reader.ReadUInt32();
            VertexCount = reader.ReadUInt16();
            _SubmeshIndex = reader.ReadUInt16();
            _SubmeshCount = reader.ReadUInt16();
            var stride = reader.ReadByte();
            reader.ReadByte(); // padding

            if( stride != 8 ) Dalamud.Log( $"Terrain Shadow: stride={stride}" );

            SubmeshView = new( "Sub-Mesh", Submeshes, false );
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

            return [.. data];
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

        public void Populate( MdlFileData data, BinaryReader reader, int lod ) {
            Lod = lod;

            PopulateIndexData( data, reader, lod );

            reader.BaseStream.Position = data.VertexBufferOffsets[lod] + _VertexBufferOffset;
            RawVertexData = reader.ReadBytes( VertexCount * 8 );

            Submeshes.AddRange( data.TerrainShadowSubmeshes.GetRange( _SubmeshIndex, _SubmeshCount ) );
            foreach( var submesh in Submeshes ) submesh.Populate( this, reader, data.IndexBufferOffsets[lod] );
        }

        public void PopulateWrite( MdlWriteData data, int lod ) {
            data.TerrainShadowMeshes.Add( this );
            data.AddVertexData( this, RawVertexData, RawIndexData, lod );
            foreach( var item in Submeshes ) item.PopulateWrite( data );
        }

        public void Write( BinaryWriter writer, MdlWriteData data ) {
            writer.Write( IndexCount );

            var offsets = data.TerrainShadowOffsets[this];
            writer.Write( offsets.Item2 );
            writer.Write( offsets.Item1 );

            writer.Write( VertexCount );
            data.WriteIndexCount( writer, Submeshes, _SubmeshIndex );
            writer.Write( ( byte )8 ); // stride
            writer.Write( ( byte )0 ); // padding
        }
    }
}
