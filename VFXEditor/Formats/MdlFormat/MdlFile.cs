using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.DirectX.Model;
using VfxEditor.FileManager;
using VfxEditor.Formats.MdlFormat.Bone;
using VfxEditor.Formats.MdlFormat.Bone.V6;
using VfxEditor.Formats.MdlFormat.Box;
using VfxEditor.Formats.MdlFormat.Eid;
using VfxEditor.Formats.MdlFormat.Lod;
using VfxEditor.Formats.MdlFormat.Mesh.Shape;
using VfxEditor.Formats.MdlFormat.NeckMorph;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Formats.MdlFormat.Vertex;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components.Dropdowns;
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

    public class MdlFile : FileManagerFile, IReadableFile<MdlFile> {
        public readonly ModelDeferredInstance MeshInstance = new();

        public const uint VERSION_6 = 0x01000006; // Updated dawntrail model

        private readonly uint Version;

        private readonly MdlStringTable OldStringTable = new();

        private readonly ParsedByteBool IndexBufferStreaming = new( "Index Buffer Streaming" );
        private readonly ParsedByteBool EdgeGeometry = new( "Edge Geometry" );
        private readonly ParsedFloat Radius = new( "Radius" );
        private readonly ParsedFlag<ModelFlags1> Flags1 = new( "Flags 1", size: 1 );
        private readonly ParsedFlag<ModelFlags2> Flags2 = new( "Flags 2", size: 1 );
        private readonly ParsedFloat ModelClipOutDistance = new( "Model Clip Out Distance" );
        private readonly ParsedFloat ShadowClipOutDistance = new( "Shadow Clip Out Distance" );
        private readonly ParsedByte Flags3 = new( "Flags 3" );
        private readonly ParsedByte BgChangeMaterialIndex = new( "Background Change Material Index" );
        private readonly ParsedByte BgCrestChangeMaterialIndex = new( "Background Crest Change Material Index" );
        // Number of ushorts (including padding) taken up by the bone table arrays. Recalculated on write for v6
        private readonly ParsedShort BoneTableArrayCountTotal = new( "Bone Table Array Count Total" );
        private readonly ParsedShort Unknown8 = new( "Unknown 8" );

        // https://github.com/TexTools/xivModdingFramework/blob/master/xivModdingFramework/Models/FileTypes/Mdl.cs
        public readonly List<MdlNeckMorph> NeckMorphs = [];
        private readonly CommandSplitView<MdlNeckMorph> NeckMorphView;

        // Added in 7.2, 16 bytes per entry, purpose unknown (shadow-related, appears on face models)
        private const int PATCH_72_ENTRY_SIZE = 16;
        private readonly byte[] Patch72Data;

        public readonly List<MdlEid> Eids = [];
        private readonly CommandSplitView<MdlEid> EidView;

        public readonly List<MdlLod> AllLods = [];
        public readonly List<MdlLod> UsedLods = [];
        private readonly UiDropdown<MdlLod> LodView;

        private bool ExtraLodEnabled => Flags2.Value.HasFlag( ModelFlags2.Extra_LoD );
        public readonly List<MdlExtraLod> ExtraLods = [];
        private readonly UiDropdown<MdlExtraLod> ExtraLodView;

        public readonly MdlBoneTables BoneTables;

        public readonly List<MdlShape> Shapes = []; // TODO

        private readonly byte[] Padding;
        private readonly uint StringTablePadding; // Can be weird for when exported from penumbra and other stuff

        // TODO
        private readonly MdlBoundingBox UnknownBoundingBox;
        private readonly MdlBoundingBox ModelBoundingBox;
        private readonly MdlBoundingBox WaterBoundingBox;
        private readonly MdlBoundingBox VerticalFogBoundingBox;

        private readonly List<MdlBoneBoundingBox> BoneBoundingBoxes = [];
        private readonly CommandSplitView<MdlBoneBoundingBox> BoneBoxView;

        private readonly List<MdlBoundingBox> UnknownBoundingBoxes = [];
        private readonly CommandSplitView<MdlBoundingBox> UnknownBoxView;

        public static MdlFile Read( BinaryReader reader, string sourcePath, bool verify ) => new( reader, verify );

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
                Dalamud.Log( value );
            }

            StringTablePadding = ( uint )( stringEndPos - reader.BaseStream.Position );
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
            Flags3.Read( reader );
            BgChangeMaterialIndex.Read( reader );
            BgCrestChangeMaterialIndex.Read( reader );
            var neckMorphCount = reader.ReadByte();
            BoneTableArrayCountTotal.Read( reader );
            Unknown8.Read( reader );
            var patch72Count = reader.ReadUInt16();
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
            for( var i = 0; i < attributeCount; i++ ) data.StringTable.AttributeStrings.Add( data.OffsetToString[reader.ReadUInt32()] );
            for( var i = 0; i < terrainShadowMeshCount; i++ ) data.TerrainShadowMeshes.Add( new( this, reader ) );
            for( var i = 0; i < submeshCount; i++ ) data.SubMeshes.Add( new( this, reader ) );
            for( var i = 0; i < terrainShadowSubmeshCount; i++ ) data.TerrainShadowSubmeshes.Add( new( this, reader ) );
            for( var i = 0; i < materialCount; i++ ) data.StringTable.MaterialStrings.Add( data.OffsetToString[reader.ReadUInt32()] );
            for( var i = 0; i < boneCount; i++ ) data.StringTable.BoneStrings.Add( data.OffsetToString[reader.ReadUInt32()] );

            OldStringTable = data.StringTable;

            BoneTables = Version == VERSION_6 ? new MdlBoneTablesV6( reader, boneTableCount, data ) : new MdlBoneTables( reader, boneTableCount, data );

            // // ======== SHAPES ============

            for( var i = 0; i < shapeCount; i++ ) data.Shapes.Add( new( reader, data.OffsetToString ) );
            for( var i = 0; i < shapeMeshCount; i++ ) data.ShapesMeshes.Add( new( reader ) );
            for( var i = 0; i < shapeValueCount; i++ ) data.ShapeValues.Add( new( reader ) );
            var submeshBoneMapSize = reader.ReadUInt32();
            for( var i = 0; i < submeshBoneMapSize / 2; i++ ) data.SubmeshBoneMap.Add( reader.ReadUInt16() );

            Shapes = data.Shapes; // TODO: ????

            for( var i = 0; i < neckMorphCount; i++ ) NeckMorphs.Add( new( this, reader ) );
            Patch72Data = reader.ReadBytes( patch72Count * PATCH_72_ENTRY_SIZE );

            Padding = reader.ReadBytes( reader.ReadByte() );

            // ======== BOXES ===============

            UnknownBoundingBox = new( reader );
            ModelBoundingBox = new( reader );
            WaterBoundingBox = new( reader );
            VerticalFogBoundingBox = new( reader );
            for( var i = 0; i < data.StringTable.BoneStrings.Count; i++ ) BoneBoundingBoxes.Add( new( data.StringTable.BoneStrings[i], reader ) );
            for( var i = 0; i < unknownCount; i++ ) UnknownBoundingBoxes.Add( new( reader ) );

            // ===== POPULATE =======

            foreach( var shape in data.Shapes ) shape.Populate( data );
            for( var i = 0; i < AllLods.Count; i++ ) AllLods[i].Populate( data, reader, i );
            for( var i = 0; i < ExtraLods.Count; i++ ) ExtraLods[i].Populate( data, reader, i ); // TODO: should this use vertexOffsets[i]?

            // ====== VIEWS ============

            EidView = new( "Bind Point", Eids, false,
                ( item, idx ) => $"Bind Point {item.ElementId.Value} (" + ( string.IsNullOrEmpty( item.ParentBone.Value ) ? "NONE" : item.ParentBone.Value ) + ")",
                () => new() );
            LodView = new( "Level of Detail", UsedLods );
            ExtraLodView = new( "Level of Detail", ExtraLods );
            UnknownBoxView = new( "Bounding Box", UnknownBoundingBoxes, false, null, () => new() );
            NeckMorphView = new( "Neck Morph", NeckMorphs, false, null, () => new( this ) );
            BoneBoxView = new( "Bounding Box", BoneBoundingBoxes, false,
                ( item, idx ) => string.IsNullOrEmpty( item.Name.Value ) ? $"Bounding Box {idx}" : item.Name.Value,
                () => new() );

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes() );
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
                if( tab ) BoneTables.Draw();
            }

            using( var tab = ImRaii.TabItem( "Bounding Boxes" ) ) {
                if( tab ) DrawBoxes();
            }

            using( var tab = ImRaii.TabItem( "Neck Morphs" ) ) {
                if( tab ) NeckMorphView.Draw();
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
            Flags3.Draw();
            BgChangeMaterialIndex.Draw();
            BgCrestChangeMaterialIndex.Draw();
            using( var _ = ImRaii.Disabled( Version == VERSION_6 ) ) {
                BoneTableArrayCountTotal.Draw();
            }
            Unknown8.Draw();
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
            var data = new MdlWriteData( this, OldStringTable );

            writer.Write( Version );
            writer.Write( data.Meshes.Count * 136 ); // stack size
            var runtimePlaceholder = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder: runtime size
            writer.Write( ( ushort )data.Meshes.Count ); // vertex declaration count
            writer.Write( ( ushort )data.StringTable.MaterialStrings.Count );

            var placeholders = writer.BaseStream.Position;
            for( var i = 0; i < 12; i++ ) writer.Write( 0 ); // 3 x vertex offsets, 3 x index offsets, 3 x vertex size, 3 x index size

            writer.Write( ( byte )UsedLods.Count );
            IndexBufferStreaming.Write( writer );
            EdgeGeometry.Write( writer );
            writer.Write( ( byte )0 ); // padding

            foreach( var mesh in data.Meshes ) mesh.Format.Write( writer );

            writer.Write( ( ushort )data.AllStrings.Count );
            writer.Write( ( ushort )0 ); // padding

            writer.Write( data.TotalStringLength + StringTablePadding );
            foreach( var item in data.AllStrings ) FileUtils.WriteString( writer, item, true );
            FileUtils.Pad( writer, StringTablePadding );

            Radius.Write( writer );
            writer.Write( ( ushort )data.Meshes.Count );
            writer.Write( ( ushort )data.StringTable.AttributeStrings.Count );
            writer.Write( ( ushort )data.SubMeshes.Count );
            writer.Write( ( ushort )data.StringTable.MaterialStrings.Count );
            writer.Write( ( ushort )data.StringTable.BoneStrings.Count );
            writer.Write( ( ushort )BoneTables.Tables.Count );
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
            Flags3.Write( writer );
            BgChangeMaterialIndex.Write( writer );
            BgCrestChangeMaterialIndex.Write( writer );
            writer.Write( ( byte )NeckMorphs.Count );
            var boneTableArrayCountTotalPosition = writer.BaseStream.Position;
            BoneTableArrayCountTotal.Write( writer ); // fixed up below for v6
            Unknown8.Write( writer );
            writer.Write( ( ushort )( Patch72Data.Length / PATCH_72_ENTRY_SIZE ) );
            FileUtils.Pad( writer, 6 ); // Padding

            foreach( var item in Eids ) item.Write( writer, data );
            foreach( var item in AllLods ) item.Write( writer, data );
            foreach( var item in ExtraLods ) item.Write( writer, data );

            foreach( var item in data.Meshes ) item.Write( writer, data );
            foreach( var item in data.StringTable.AttributeStrings ) writer.Write( data.StringToOffset[item] );
            foreach( var item in data.TerrainShadowMeshes ) item.Write( writer, data );
            foreach( var item in data.SubMeshes ) item.Write( writer, data );
            foreach( var item in data.TerrainShadowSubmeshes ) item.Write( writer );
            foreach( var item in data.StringTable.MaterialStrings ) writer.Write( data.StringToOffset[item] );
            foreach( var item in data.StringTable.BoneStrings ) writer.Write( data.StringToOffset[item] );

            var boneTablesStart = writer.BaseStream.Position;
            BoneTables.Write( writer, data );

            if( Version == VERSION_6 ) {
                // Arrays start after the (offset, count) header of each table
                var boneTablesEnd = writer.BaseStream.Position;
                var arraysStart = boneTablesStart + ( BoneTables.Tables.Count * 4 );
                BoneTableArrayCountTotal.Value = ( int )( ( boneTablesEnd - arraysStart ) / 2 );

                writer.BaseStream.Position = boneTableArrayCountTotalPosition;
                BoneTableArrayCountTotal.Write( writer );
                writer.BaseStream.Position = boneTablesEnd;
            }

            foreach( var item in data.Shapes ) item.Write( writer, data );
            foreach( var item in data.ShapesMeshes ) item.Write( writer, data );
            foreach( var item in data.ShapeValues ) item.Write( writer );

            var boneMapPadding = ( uint )FileUtils.NumberToPad( writer.BaseStream.Position + ( data.SubmeshBoneMap.Count * 2 ), 4 );
            writer.Write( ( uint )data.SubmeshBoneMap.Count * 2 + boneMapPadding );
            foreach( var item in data.SubmeshBoneMap ) writer.Write( item );
            FileUtils.Pad( writer, boneMapPadding );

            foreach( var item in NeckMorphs ) item.Write( writer );
            writer.Write( Patch72Data );

            writer.Write( ( byte )Padding.Length );
            writer.Write( Padding );

            UnknownBoundingBox.Write( writer );
            ModelBoundingBox.Write( writer );
            WaterBoundingBox.Write( writer );
            VerticalFogBoundingBox.Write( writer );

            foreach( var bone in data.StringTable.BoneStrings ) {
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
                writer.Write( item.Unknown );

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