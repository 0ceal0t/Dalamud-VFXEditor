using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
using VfxEditor.Formats.MdlFormat.Mesh.Shape;
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
        public int Lod { get; private set; } = -1;
        public readonly MdlVertexDeclaration Format;

        private readonly byte[] Strides;
        private readonly byte StreamCount;

        private readonly ushort _MaterialStringIdx;
        private readonly ushort _SubmeshIndex;
        private readonly ushort _SubmeshCount;
        private readonly uint[] _VertexBufferOffsets;

        private readonly ParsedString Material = new( "Material" );
        private readonly ParsedInt BoneTableIndex = new( "Bone Table Index" );

        private readonly ushort VertexCount; // Maxes out at ushort.MaxValue

        private List<byte[]> RawVertexData = [[], [], []];

        private readonly List<MdlSubMesh> Submeshes = [];
        private readonly UiSplitView<MdlSubMesh> SubmeshView;

        private readonly List<MdlShapeMesh> Shapes = [];

        public MdlMesh( MdlFile file, MdlVertexDeclaration format, BinaryReader reader ) : base( file ) {
            Format = format;

            VertexCount = reader.ReadUInt16();
            reader.ReadUInt16(); // padding
            IndexCount = reader.ReadUInt32();
            _MaterialStringIdx = reader.ReadUInt16();
            _SubmeshIndex = reader.ReadUInt16();
            _SubmeshCount = reader.ReadUInt16();

            var _boneTableIndex = reader.ReadUInt16();
            BoneTableIndex.Value = _boneTableIndex == 255 ? -1 : _boneTableIndex;

            _IndexOffset = 2 * reader.ReadUInt32();
            _VertexBufferOffsets = [reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32()];
            Strides = reader.ReadBytes( 3 );
            StreamCount = reader.ReadByte();

            SubmeshView = new( "Sub-Mesh", Submeshes, false );
        }

        public override Vector4[] GetData( int indexCount, byte[] rawIndexData ) => Format.GetData( rawIndexData, RawVertexData, indexCount, VertexCount, Strides );

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Mesh" ) ) {
                if( tab ) DrawMesh();
            }

            using( var tab = ImRaii.TabItem( "Sub-Meshes" ) ) {
                if( tab ) SubmeshView.Draw();
            }
        }

        private void DrawMesh() {
            BoneTableIndex.Draw();
            Material.Draw();

            using( var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                var names = Shapes.Select( x => x.Name ).Where( x => !string.IsNullOrEmpty( x ) ).ToList();
                for( var i = 0; i < names.Count; i++ ) {
                    if( i > 0 ) ImGui.SameLine();
                    using var _ = ImRaii.PushId( i );
                    using var disabled = ImRaii.Disabled();
                    ImGui.SmallButton( names[i] );
                }
            }

            DrawPreview();
        }

        public void Populate( MdlFileData data, BinaryReader reader, int lod ) {
            Lod = lod;

            PopulateIndexData( data, reader, lod );

            RawVertexData = [];
            for( var i = 0; i < 3; i++ ) {
                var stride = Strides[i];
                if( stride == 0 ) continue;
                reader.BaseStream.Position = data.VertexBufferOffsets[lod] + _VertexBufferOffsets[i];
                RawVertexData.Add( reader.ReadBytes( VertexCount * stride ) );
            }

            Submeshes.AddRange( data.SubMeshes.GetRange( _SubmeshIndex, _SubmeshCount ) );
            foreach( var submesh in Submeshes ) submesh.Populate( this, data, reader, data.IndexBufferOffsets[lod] );

            Material.Value = data.StringTable.MaterialStrings[_MaterialStringIdx];

            foreach( var shape in data.Shapes ) Shapes.AddRange( shape.ShapeMeshes[lod].Where( x => x._MeshIndexOffset == _IndexOffset / 2 ) );
        }

        public void PopulateWrite( MdlWriteData data, int lod ) {
            data.Meshes.Add( this );
            data.AddMaterial( Material.Value );
            data.AddVertexData( this, RawVertexData, RawIndexData, lod );
            foreach( var item in Submeshes ) item.PopulateWrite( data );
        }

        public void Write( BinaryWriter writer, MdlWriteData data ) {
            writer.Write( VertexCount );
            writer.Write( ( ushort )0 ); // padding
            writer.Write( IndexCount );
            writer.Write( ( ushort )data.StringTable.MaterialStrings.IndexOf( Material.Value ) );
            data.WriteIndexCount( writer, Submeshes, _SubmeshIndex );
            writer.Write( ( ushort )( BoneTableIndex.Value == -1 ? 255 : BoneTableIndex.Value ) );

            var offsets = data.MeshOffsets[this];
            writer.Write( offsets.Item2 ); // index offset
            writer.Write( offsets.Item1[0] ); // vertex offsets
            writer.Write( offsets.Item1[1] );
            writer.Write( offsets.Item1[2] );

            writer.Write( Strides );
            writer.Write( StreamCount );
        }
    }
}
