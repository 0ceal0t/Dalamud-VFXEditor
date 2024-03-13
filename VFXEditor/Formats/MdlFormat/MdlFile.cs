using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Formats.MdlFormat.Bone;
using VfxEditor.Formats.MdlFormat.Box;
using VfxEditor.Formats.MdlFormat.Element;
using VfxEditor.Formats.MdlFormat.Lod;
using VfxEditor.Formats.MdlFormat.Mesh.Shape;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Formats.MdlFormat.Vertex;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MdlFormat {
    // https://github.com/Caraxi/SimpleHeels/blob/d345d7406958e70fb3c6823cee21872ffa65621b/Files/MdlFile.cs#L291
    // https://github.com/NotAdam/Lumina/blob/40dab50183eb7ddc28344378baccc2d63ae71d35/src/Lumina/Data/Files/MdlFile.cs#L7

    [Flags]
    public enum ModelFlags1 : int {
        Dust_Occlusion = 0x80,
        Snow_Occlusion = 0x40,
        Rain_Occlusion = 0x20,
        Unknown_1 = 0x10,
        Lighting_Reflection = 0x08,
        Waving_Animation_Disabled = 0x04,
        Light_Shadow_Disabled = 0x02,
        Shadow_Disabled = 0x01,
    }

    [Flags]
    public enum ModelFlags2 : int {
        Unknown_3 = 0x80,
        Background_UV_Scroll = 0x40,
        Force_NonResident = 0x20,
        Extra_LoD = 0x10,
        Shadow_Mask = 0x08,
        Force_LoD_Range = 0x04,
        Edge_Geometry = 0x02,
        Unknown_2 = 0x01
    }

    public class MdlFile : FileManagerFile {
        private readonly uint Version;

        private readonly ParsedByteBool IndexBufferStreaming = new( "Index Buffer Streaming" );
        private readonly ParsedByteBool EdgeGeometry = new( "Edge Geometry" );
        private readonly ParsedFloat Radius = new( "Radius" );
        private readonly ParsedFlag<ModelFlags1> Flags1 = new( "Flags 1", size: 1 );
        private readonly ParsedFlag<ModelFlags2> Flags2 = new( "Flags 2", size: 1 );
        private readonly ParsedFloat ModelClipOutDistance = new( "Model Clip Out Distance" );
        private readonly ParsedFloat ShadowClipOutDistance = new( "Shadow Clip Out Distance" );
        private readonly ParsedByte Unknown5 = new( "Unknown 5" );
        private readonly ParsedByte BgChangeMaterialIndex = new( "Background Change Material Index" );
        private readonly ParsedByte BgCrestChangeMaterialIndex = new( "Background Crest Change Material Index" );
        private readonly ParsedByte Unknown6 = new( "Unknown 6" );
        private readonly ParsedShort Unknown7 = new( "Unknown 7" );
        private readonly ParsedShort Unknown8 = new( "Unknown 8" );
        private readonly ParsedShort Unknown9 = new( "Unknown 9" );

        public readonly List<MdlEid> Eids = new();
        private readonly CommandSplitView<MdlEid> EidView;

        public readonly List<MdlLod> AllLods = new();
        public readonly List<MdlLod> UsedLods = new();
        private readonly UiDropdown<MdlLod> LodView;

        private bool ExtraLodEnabled => Flags2.Value.HasFlag( ModelFlags2.Extra_LoD );
        public readonly List<MdlExtraLod> ExtraLods = new();
        private readonly UiDropdown<MdlExtraLod> ExtraLodView;

        public readonly List<MdlBoneTable> BoneTables = new();
        private readonly UiSplitView<MdlBoneTable> BoneTableView;

        public readonly List<MdlShape> Shapes = new(); // TODO

        private readonly byte[] Padding;

        // TODO
        private readonly MdlBoundingBox UnknownBoundingBox;
        private readonly MdlBoundingBox ModelBoundingBox;
        private readonly MdlBoundingBox WaterBoundingBox;
        private readonly MdlBoundingBox VerticalFogBoundingBox;

        private readonly List<MdlBoneBoundingBox> BoneBoundingBoxes = new();
        private readonly CommandSplitView<MdlBoneBoundingBox> BoneBoxView;

        private readonly List<MdlBoundingBox> UnknownBoundingBoxes = new();
        private readonly CommandSplitView<MdlBoundingBox> UnknownBoxView;

        public MdlFile( BinaryReader reader, bool verify ) : base() {
            var data = new MdlFileData();

            Version = reader.ReadUInt32();
            reader.ReadUInt32(); // stack size
            reader.ReadUInt32(); // runtime size
            var vertexDeclarationCount = reader.ReadUInt16();
            var _materialCount = reader.ReadUInt16();

            // Order of the data is: V1, I1, V2, I2, V3, I3
            for( var i = 0; i < 3; i++ ) data.VertexBufferOffsets.Add( reader.ReadUInt32() );
            for( var i = 0; i < 3; i++ ) data.IndexBufferOffsets.Add( reader.ReadUInt32() );
            for( var i = 0; i < 3; i++ ) data.VertexBufferSizes.Add( reader.ReadUInt32() );
            for( var i = 0; i < 3; i++ ) data.IndexBufferSizes.Add( reader.ReadUInt32() );

            var _lodCount = reader.ReadByte(); // sometimes != 3, such as with bg/ffxiv/zon_z1/chr/z1c4/bgplate/0001.mdl
            IndexBufferStreaming.Read( reader );
            EdgeGeometry.Read( reader );
            reader.ReadByte(); // padding

            // ===== VERTEX DECLARATION ======

            var vertexFormats = new List<MdlVertexDeclaration>();
            for( var i = 0; i < vertexDeclarationCount; i++ ) vertexFormats.Add( new( reader ) );

            // ====== STRINGS ======

            var stringCount = reader.ReadUInt16();
            reader.ReadUInt16(); // padding
            var stringSize = reader.ReadUInt32();
            var stringStartPos = reader.BaseStream.Position;
            var stringEndPos = reader.BaseStream.Position + stringSize;

            for( var i = 0; i < stringCount; i++ ) {
                var pos = reader.BaseStream.Position - stringStartPos;
                var value = FileUtils.ReadString( reader );
                data.OffsetToString[( uint )pos] = value;
            }

            reader.BaseStream.Position = stringEndPos;

            // ====== MODEL HEADER =======

            Radius.Read( reader );
            var meshCount = reader.ReadUInt16();
            var attributeCount = reader.ReadUInt16();
            var submeshCount = reader.ReadUInt16();
            var materialCount = reader.ReadUInt16();
            var boneCount = reader.ReadUInt16();
            var boneTableCount = reader.ReadUInt16();
            var shapeCount = reader.ReadUInt16();
            var shapeMeshCount = reader.ReadUInt16();
            var shapeValueCount = reader.ReadUInt16();
            var lodCount = reader.ReadByte();
            Flags1.Read( reader );
            var elementIdCount = reader.ReadUInt16();
            var terrainShadowMeshCount = reader.ReadByte();
            Flags2.Read( reader );
            ModelClipOutDistance.Read( reader );
            ShadowClipOutDistance.Read( reader );
            var unknownCount = reader.ReadUInt16();
            var terrainShadowSubmeshCount = reader.ReadUInt16();
            Unknown5.Read( reader );
            BgChangeMaterialIndex.Read( reader );
            BgCrestChangeMaterialIndex.Read( reader );
            Unknown6.Read( reader );
            Unknown7.Read( reader );
            Unknown8.Read( reader );
            Unknown9.Read( reader );
            reader.ReadBytes( 6 ); // padding

            if( meshCount != vertexDeclarationCount || materialCount != _materialCount || _lodCount != lodCount ) {
                Dalamud.Error( $"Mesh:{meshCount}/{vertexDeclarationCount} Material:{materialCount}/{_materialCount} LoD:{lodCount}/{_lodCount}" );
            }

            // ====== DATA ========

            for( var i = 0; i < elementIdCount; i++ ) Eids.Add( new( data.OffsetToString, reader ) );

            // ====== LOD ========

            for( var i = 0; i < 3; i++ ) {
                var lod = new MdlLod( this, reader );
                AllLods.Add( lod );
                if( i < _lodCount ) UsedLods.Add( lod );
            }

            if( ExtraLodEnabled ) {
                Dalamud.Error( "Extra LoD" );
                for( var i = 0; i < 3; i++ ) ExtraLods.Add( new( this, reader ) );
            }

            // ===== MESHES ========

            for( var i = 0; i < meshCount; i++ ) data.Meshes.Add( new( this, vertexFormats[i], reader ) );
            for( var i = 0; i < attributeCount; i++ ) data.AttributeStrings.Add( data.OffsetToString[reader.ReadUInt32()] );
            for( var i = 0; i < terrainShadowMeshCount; i++ ) data.TerrainShadowMeshes.Add( new( this, reader ) );
            for( var i = 0; i < submeshCount; i++ ) data.SubMeshes.Add( new( reader ) );
            for( var i = 0; i < terrainShadowSubmeshCount; i++ ) data.TerrainShadowSubmeshes.Add( new( reader ) );
            for( var i = 0; i < materialCount; i++ ) data.MaterialStrings.Add( data.OffsetToString[reader.ReadUInt32()] );
            for( var i = 0; i < boneCount; i++ ) data.BoneStrings.Add( data.OffsetToString[reader.ReadUInt32()] );
            for( var i = 0; i < boneTableCount; i++ ) BoneTables.Add( new( reader, data.BoneStrings ) );

            // // ======== SHAPES ============

            for( var i = 0; i < shapeCount; i++ ) data.Shapes.Add( new( reader, data.OffsetToString ) );
            for( var i = 0; i < shapeMeshCount; i++ ) data.ShapesMeshes.Add( new( reader ) );
            for( var i = 0; i < shapeValueCount; i++ ) data.ShapeValues.Add( new( reader ) );
            var submeshBoneMapSize = reader.ReadUInt32();
            for( var i = 0; i < submeshBoneMapSize / 2; i++ ) data.SubmeshBoneMap.Add( reader.ReadUInt16() );

            Shapes = data.Shapes; // TODO: ????

            Padding = reader.ReadBytes( reader.ReadByte() );

            // ======== BOXES ===============

            UnknownBoundingBox = new( reader );
            ModelBoundingBox = new( reader );
            WaterBoundingBox = new( reader );
            VerticalFogBoundingBox = new( reader );
            for( var i = 0; i < data.BoneStrings.Count; i++ ) BoneBoundingBoxes.Add( new( data.BoneStrings[i], reader ) );
            for( var i = 0; i < unknownCount; i++ ) UnknownBoundingBoxes.Add( new( reader ) );

            // ===== POPULATE =======

            foreach( var shape in data.Shapes ) shape.Populate( data );
            for( var i = 0; i < AllLods.Count; i++ ) AllLods[i].Populate( data, reader, i );
            for( var i = 0; i < ExtraLods.Count; i++ ) ExtraLods[i].Populate( data, reader, i ); // TODO: should this use vertexOffsets[i]?

            // ====== VIEWS ============

            EidView = new( "Bind Point", Eids, false,
                ( MdlEid item, int idx ) => $"Bind Point {item.ElementId.Value} (" + ( string.IsNullOrEmpty( item.ParentBone.Value ) ? "NONE" : item.ParentBone.Value ) + ")",
                () => new() );
            LodView = new( "Level of Detail", UsedLods );
            ExtraLodView = new( "Level of Detail", ExtraLods );
            BoneTableView = new( "Bone Table", BoneTables, false );
            UnknownBoxView = new( "Bounding Box", UnknownBoundingBoxes, false, null, () => new() );
            BoneBoxView = new( "Bounding Box", BoneBoundingBoxes, false,
                ( MdlBoneBoundingBox item, int idx ) => string.IsNullOrEmpty( item.Name.Value ) ? $"Bounding Box {idx}" : item.Name.Value,
                () => new() );

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes(), null );
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) DrawParameters();
            }

            using( var tab = ImRaii.TabItem( "Bind Points" ) ) {
                if( tab ) EidView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Levels of Detail" ) ) {
                if( tab ) LodView.Draw();
            }

            if( ExtraLodEnabled ) {
                using var tab = ImRaii.TabItem( "Extra LoD" );
                if( tab ) ExtraLodView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Bone Tables" ) ) {
                if( tab ) BoneTableView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Bounding Boxes" ) ) {
                if( tab ) DrawBoxes();
            }
        }

        private void DrawParameters() {
            using var child = ImRaii.Child( "Child" );

            IndexBufferStreaming.Draw();
            EdgeGeometry.Draw();
            Radius.Draw();
            Flags1.Draw();
            Flags2.Draw(); // Not gonna handle if ExtraLoD is checked :/
            ModelClipOutDistance.Draw();
            ShadowClipOutDistance.Draw();
            Unknown5.Draw();
            BgChangeMaterialIndex.Draw();
            BgCrestChangeMaterialIndex.Draw();
            Unknown6.Draw();
            Unknown7.Draw();
            Unknown8.Draw();
            Unknown9.Draw();
        }

        private void DrawBoxes() {
            using var id = ImRaii.PushId( "Boxes" );
            using var child = ImRaii.Child( "Child" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Bones" ) ) {
                if( tab ) BoneBoxView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Unknown" ) ) {
                if( tab ) UnknownBoundingBox.Draw();
            }

            using( var tab = ImRaii.TabItem( "Model" ) ) {
                if( tab ) ModelBoundingBox.Draw();
            }

            using( var tab = ImRaii.TabItem( "Vertical Fog" ) ) {
                if( tab ) VerticalFogBoundingBox.Draw();
            }

            using( var tab = ImRaii.TabItem( "Unknown Boxes" ) ) {
                if( tab ) UnknownBoxView.Draw();
            }
        }

        public override void Write( BinaryWriter writer ) {
            var data = new MdlWriteData( this );

            writer.Write( Version );
            writer.Write( data.Meshes.Count * 136 ); // stack size
            var runtimePlaceholder = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder: runtime size
            writer.Write( ( ushort )data.Meshes.Count ); // vertex declaration count
            writer.Write( ( ushort )data.MaterialStrings.Count );

            var placeholders = writer.BaseStream.Position;
            for( var i = 0; i < 12; i++ ) writer.Write( 0 ); // 3 x vertex offsets, 3 x index offsets, 3 x vertex size, 3 x index size

            writer.Write( ( byte )UsedLods.Count );
            IndexBufferStreaming.Write( writer );
            EdgeGeometry.Write( writer );
            writer.Write( ( byte )0 ); // padding

            foreach( var mesh in data.Meshes ) mesh.Format.Write( writer );

            writer.Write( ( ushort )data.AllStrings.Count );
            writer.Write( ( ushort )0 ); // padding

            var stringPadding = ( uint )FileUtils.NumberToPad( writer.BaseStream.Position + data.TotalStringLength, 4 );
            writer.Write( data.TotalStringLength + stringPadding );
            foreach( var item in data.AllStrings ) FileUtils.WriteString( writer, item, true );
            FileUtils.Pad( writer, stringPadding );

            Radius.Write( writer );
            writer.Write( ( ushort )data.Meshes.Count );
            writer.Write( ( ushort )data.AttributeStrings.Count );
            writer.Write( ( ushort )data.SubMeshes.Count );
            writer.Write( ( ushort )data.MaterialStrings.Count );
            writer.Write( ( ushort )data.BoneStrings.Count );
            writer.Write( ( ushort )BoneTables.Count );
            writer.Write( ( ushort )data.Shapes.Count );
            writer.Write( ( ushort )data.ShapesMeshes.Count );
            writer.Write( ( ushort )data.ShapeValues.Count );
            writer.Write( ( byte )UsedLods.Count );
            Flags1.Write( writer );
            writer.Write( ( ushort )Eids.Count );
            writer.Write( ( byte )data.TerrainShadowMeshes.Count );
            Flags2.Write( writer );
            ModelClipOutDistance.Write( writer );
            ShadowClipOutDistance.Write( writer );
            writer.Write( ( ushort )UnknownBoundingBoxes.Count );
            writer.Write( ( ushort )data.TerrainShadowSubmeshes.Count );
            Unknown5.Write( writer );
            BgChangeMaterialIndex.Write( writer );
            BgCrestChangeMaterialIndex.Write( writer );
            Unknown6.Write( writer );
            Unknown7.Write( writer );
            Unknown8.Write( writer );
            Unknown9.Write( writer );
            FileUtils.Pad( writer, 6 ); // Padding

            foreach( var item in Eids ) item.Write( writer, data );
            foreach( var item in AllLods ) item.Write( writer, data );
            foreach( var item in ExtraLods ) item.Write( writer, data );

            foreach( var item in data.Meshes ) item.Write( writer, data );
            foreach( var item in data.AttributeStrings ) writer.Write( data.StringToOffset[item] );
            foreach( var item in data.TerrainShadowMeshes ) item.Write( writer, data );
            foreach( var item in data.SubMeshes ) item.Write( writer, data );
            foreach( var item in data.TerrainShadowSubmeshes ) item.Write( writer );
            foreach( var item in data.MaterialStrings ) writer.Write( data.StringToOffset[item] );
            foreach( var item in data.BoneStrings ) writer.Write( data.StringToOffset[item] );
            foreach( var item in BoneTables ) item.Write( writer, data );

            foreach( var item in data.Shapes ) item.Write( writer, data );
            foreach( var item in data.ShapesMeshes ) item.Write( writer, data );
            foreach( var item in data.ShapeValues ) item.Write( writer );

            var boneMapPadding = ( uint )FileUtils.NumberToPad( writer.BaseStream.Position + ( data.SubmeshBoneMap.Count * 2 ), 4 );
            writer.Write( ( uint )data.SubmeshBoneMap.Count * 2 + boneMapPadding );
            foreach( var item in data.SubmeshBoneMap ) writer.Write( item );
            FileUtils.Pad( writer, boneMapPadding );

            writer.Write( ( byte )Padding.Length );
            writer.Write( Padding );

            UnknownBoundingBox.Write( writer );
            ModelBoundingBox.Write( writer );
            WaterBoundingBox.Write( writer );
            VerticalFogBoundingBox.Write( writer );

            foreach( var bone in data.BoneStrings ) {
                if( BoneBoundingBoxes.FindFirst( x => x.Name.Value.Equals( bone ), out var box ) ) {
                    box.Write( writer );
                    continue;
                }
                FileUtils.Pad( writer, 32 ); // Couldn't find one, just skip it
            }

            foreach( var item in UnknownBoundingBoxes ) item.Write( writer );

            // ================

            var vertexSizes = new List<uint>();
            var indexSizes = new List<uint>();
            var vertexOffsets = new List<uint>();
            var indexOffsets = new List<uint>();

            for( var i = 0; i < 3; i++ ) {
                vertexSizes.Add( ( uint )data.VertexData[i].Length );
                vertexOffsets.Add( vertexSizes[i] == 0 ? 0 : ( uint )writer.BaseStream.Position );
                writer.Write( data.VertexData[i].ToArray() );

                indexSizes.Add( ( uint )( data.IndexData[i].Length ) );
                indexOffsets.Add( indexSizes[i] == 0 ? 0 : ( uint )writer.BaseStream.Position );
                writer.Write( data.IndexData[i].ToArray() );
            }

            // ===== FILL IN PLACEHOLDERS =======

            writer.BaseStream.Position = runtimePlaceholder;
            var runtimeSize = vertexOffsets[0] - 68u - ( uint )( data.Meshes.Count * 136u );
            writer.Write( runtimeSize );

            writer.BaseStream.Position = placeholders;
            foreach( var item in vertexOffsets ) writer.Write( item );
            foreach( var item in indexOffsets ) writer.Write( item );
            foreach( var item in vertexSizes ) writer.Write( item );
            foreach( var item in indexSizes ) writer.Write( item );

            foreach( var (item, idx) in AllLods.WithIndex() ) {
                writer.BaseStream.Position = data.LodPlaceholders[item];
                writer.Write( 0 );
                writer.Write( vertexOffsets[idx] + vertexSizes[idx] );
                writer.Write( 0 );
                writer.Write( 0 );

                writer.Write( vertexSizes[idx] );
                writer.Write( indexSizes[idx] );
                writer.Write( vertexOffsets[idx] );
                writer.Write( indexOffsets[idx] );
            }

            // ===============

            data.Dispose();
        }
    }
}
