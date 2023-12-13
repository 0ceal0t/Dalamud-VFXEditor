using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.SgbFormat.Layers.Objects;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbFormat.Layers {
    public class SgbLayer : IUiItem {
        public readonly ParsedUInt Id = new( "Layer Id" );
        public readonly ParsedString Name = new( "Name" );
        private readonly ParsedByteBool ToolModeVisible = new( "Tool Mode Visible" );
        private readonly ParsedByteBool ToolModeReadOly = new( "Tool Mode Readonly" );
        private readonly ParsedByteBool IsBushLayer = new( "Is Bush Layer" );
        private readonly ParsedByteBool PS3Visible = new( "PS3 Visible" );
        private readonly ParsedShort FestivalId = new( "Festival Id" );
        private readonly ParsedShort FestivalPhaseId = new( "Festival Phase Id" );
        private readonly ParsedByteBool IsTemporary = new( "Is Temporary" );
        private readonly ParsedByteBool IsHousing = new( "Is Housing" );
        private readonly ParsedShort VersionMask = new( "Version Mask" );

        private readonly List<SgbObject> Objects = new();
        private readonly SgbObjectSplitView ObjectView;

        public SgbLayer() {
            ObjectView = new( Objects );
        }

        public SgbLayer( BinaryReader reader ) : this() {
            // https://github.com/NotAdam/Lumina/blob/40dab50183eb7ddc28344378baccc2d63ae71d35/src/Lumina/Data/Parsing/Layer/LayerCommon.cs#L1596
            var startPos = reader.BaseStream.Position;

            Id.Read( reader );
            Name.Value = FileUtils.ReadStringOffset( startPos, reader );
            var instanceObjects = reader.ReadUInt32();
            var instanceObjectCount = reader.ReadUInt32();
            ToolModeVisible.Read( reader );
            ToolModeReadOly.Read( reader );
            IsBushLayer.Read( reader );
            PS3Visible.Read( reader );
            var layerSetReferenceList = reader.ReadUInt32();
            FestivalId.Read( reader );
            FestivalPhaseId.Read( reader );
            IsTemporary.Read( reader );
            IsHousing.Read( reader );
            VersionMask.Read( reader );
            reader.ReadInt32(); // padding
            var obSet = reader.ReadUInt32();
            var obSetCount = reader.ReadUInt32();
            var obSetEnabled = reader.ReadUInt32();
            var obSetEnabledCount = reader.ReadUInt32();

            foreach( var offset in FileUtils.ReadOffsets( instanceObjectCount, startPos + instanceObjects, reader ) ) {
                reader.BaseStream.Seek( startPos + instanceObjects + offset, SeekOrigin.Begin );
                var type = ( LayerEntryType )reader.ReadInt32();
                if( !SgbObjectUtils.ObjectTypes.TryGetValue( type, out var objectType ) ) {
                    Dalamud.Error( $"Unknown object type {type}" );
                    continue;
                }

                var constructor = objectType.GetConstructor( new Type[] { typeof( LayerEntryType ), typeof( BinaryReader ) } );
                var newEntry = ( SgbObject )constructor.Invoke( new object[] { type, reader } );
                Objects.Add( newEntry );
            }

            reader.BaseStream.Seek( startPos + layerSetReferenceList, SeekOrigin.Begin );
            // Read list

            reader.BaseStream.Seek( startPos + obSet, SeekOrigin.Begin );
            for( var i = 0; i < obSetCount; i++ ) {
                // Read OBSet
            }

            reader.BaseStream.Seek( startPos + obSetEnabled, SeekOrigin.Begin );
            for( var i = 0; i < obSetEnabledCount; i++ ) {
                // Read OBSetEnabled
            }
        }

        public void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) DrawParameters();
            }

            using( var tab = ImRaii.TabItem( "Objects" ) ) {
                if( tab ) ObjectView.Draw();
            }

            // TODO
        }

        private void DrawParameters() {
            Id.Draw();
            Name.Draw();
            ToolModeVisible.Draw();
            ToolModeReadOly.Draw();
            IsBushLayer.Draw();
            PS3Visible.Draw();
            FestivalId.Draw();
            FestivalPhaseId.Draw();
            IsTemporary.Draw();
            IsHousing.Draw();
            VersionMask.Draw();
        }
    }
}
