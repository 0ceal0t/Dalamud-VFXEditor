using Dalamud.Logging;
using ImGuiNET;
using Lumina.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.UldFormat.Component;
using VfxEditor.UldFormat.Component.Node;
using VfxEditor.UldFormat.Headers;
using VfxEditor.UldFormat.PartList;
using VfxEditor.UldFormat.Texture;
using VfxEditor.UldFormat.Timeline;
using VfxEditor.UldFormat.Widget;
using VfxEditor.Utils;

namespace VfxEditor.UldFormat {
    public class UldFile : FileManagerFile {
        private readonly UldMainHeader Header;
        private readonly UldAtkHeader OffsetsHeader;
        private readonly UldAtkHeader2 OffsetsHeader2;

        private readonly UldListHeader AssetList;
        private readonly List<UldTexture> Assets = new();

        private readonly UldListHeader PartList;
        private readonly List<UldPartList> Parts = new();

        private readonly UldListHeader ComponentList;
        private readonly List<UldComponent> Components = new();

        private readonly UldListHeader TimelineList;
        private readonly List<UldTimeline> Timelines = new();

        private readonly UldListHeader WidgetList;
        private readonly List<UldWidget> Widgets = new();

        public readonly UldTextureSplitView AssetSplitView;
        public readonly UldPartsSplitView PartsSplitView;
        public readonly UldComponentDropdown ComponentDropdown;
        public readonly UldTimelineDropdown TimelineDropdown;
        public readonly UldWidgetDropdown WidgetDropdown;

        public UldFile( BinaryReader reader, bool checkOriginal = true ) : base( new CommandManager( Plugin.UldManager.GetCopyManager() ) ) {
            List<DelayedNodeData> delayed = new();

            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            var pos = reader.BaseStream.Position;
            Header = new( reader ); // uldh 0100

            var offsetsPosition = reader.BaseStream.Position;
            OffsetsHeader = new( reader ); // atkh 0100

            // ==== ASSETS ======
            if( OffsetsHeader.AssetOffset > 0 ) {
                reader.Seek( offsetsPosition + OffsetsHeader.AssetOffset );
                AssetList = new( reader );
                for( var i = 0; i < AssetList.ElementCount; i++ ) Assets.Add( new( reader, AssetList.Version[3] ) );
            }
            else AssetList = new( "ashd", "0101" );

            // ===== PARTS ======
            if( OffsetsHeader.PartOffset > 0 ) {
                reader.Seek( offsetsPosition + OffsetsHeader.PartOffset );
                PartList = new( reader );
                for( var i = 0; i < PartList.ElementCount; i++ ) Parts.Add( new( reader ) );
            }
            else PartList = new( "tphd", "0100" );

            // ====== COMPONENTS =======
            if( OffsetsHeader.ComponentOffset > 0 ) {
                reader.Seek( offsetsPosition + OffsetsHeader.ComponentOffset );
                ComponentList = new( reader );
                for( var i = 0; i < ComponentList.ElementCount; i++ ) Components.Add( new( reader, Components, delayed ) );
            }
            else ComponentList = new( "cohd", "0100" );

            // ===== TIMELINES ====
            if( OffsetsHeader.TimelineOffset > 0 ) {
                reader.Seek( offsetsPosition + OffsetsHeader.TimelineOffset );
                TimelineList = new( reader );
                for( var i = 0; i < TimelineList.ElementCount; i++ ) Timelines.Add( new( reader ) );
            }
            else TimelineList = new( "tlhd", "0100" );

            reader.Seek( pos + Header.WidgetOffset );
            var offsetsPosition2 = reader.BaseStream.Position;
            OffsetsHeader2 = new( reader );

            // ===== WIDGETS ====
            if( OffsetsHeader2.WidgetOffset > 0 ) {
                reader.Seek( offsetsPosition2 + OffsetsHeader2.WidgetOffset );
                WidgetList = new( reader );
                for( var i = 0; i < WidgetList.ElementCount; i++ ) Widgets.Add( new( reader, Components, delayed ) );
            }
            else WidgetList = new( "wdhd", "0100" );

            foreach( var item in delayed ) {
                item.Node.InitData( reader, item );
            }

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );

            AssetSplitView = new( Assets );
            PartsSplitView = new( Parts );
            ComponentDropdown = new( Components );
            TimelineDropdown = new( Timelines );
            WidgetDropdown = new( Widgets, Components );
        }

        public override void Write( BinaryWriter writer ) {
            var pos = writer.BaseStream.Position;
            Header.Write( writer, out var headerUpdatePosition );

            var offsetsPosition = writer.BaseStream.Position;
            OffsetsHeader.Write( writer, out var offsetsUpdatePosition );

            // ====== ASSETS ========
            var assetOffset = AssetList.Write( writer, Assets, offsetsPosition );
            Assets.ForEach( x => x.Write( writer, AssetList.Version[3] ) );

            // ===== PARTS ========
            var partOffset = PartList.Write( writer, Parts, offsetsPosition );
            Parts.ForEach( x => x.Write( writer ) );

            // ====== COMPONENTS ======
            var componentOffset = ComponentList.Write( writer, Components, offsetsPosition );
            Components.ForEach( x => x.Write( writer ) );

            // ====== TIMELINES =====
            var timelineOffset = TimelineList.Write( writer, Timelines, offsetsPosition );
            Timelines.ForEach( x => x.Write( writer ) );

            var headerWidgetOffset = writer.BaseStream.Position - pos;
            var offsetsPosition2 = writer.BaseStream.Position;
            OffsetsHeader2.Write( writer, out var offsetsUpdatePosition2 );

            // ====== WIDGETS ========
            var widgetOffset = WidgetList.Write( writer, Widgets, offsetsPosition2 );
            Widgets.ForEach( x => x.Write( writer ) );

            var finalPos = writer.BaseStream.Position;

            UldMainHeader.UpdateOffsets( writer, headerUpdatePosition, ( uint )headerWidgetOffset );
            UldAtkHeader.UpdateOffsets( writer, offsetsUpdatePosition, ( uint )assetOffset, ( uint )partOffset, ( uint )componentOffset, ( uint )timelineOffset );
            UldAtkHeader2.UpdateOffsets( writer, offsetsUpdatePosition2, ( uint )widgetOffset );

            writer.BaseStream.Position = finalPos;
        }

        public override void Draw( string id ) {
            if( ImGui.BeginTabBar( $"{id}-MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( $"Assets{id}" ) ) {
                    AssetList.Draw( id );
                    AssetSplitView.Draw( $"{id}/Assets" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Part Lists{id}" ) ) {
                    PartList.Draw( id );
                    PartsSplitView.Draw( $"{id}/Parts" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Components{id}" ) ) {
                    ComponentList.Draw( id );
                    ComponentDropdown.Draw( $"{id}/Components" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Timelines{id}" ) ) {
                    TimelineList.Draw( id );
                    TimelineDropdown.Draw( $"{id}/Timelines" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Widgets{id}" ) ) {
                    WidgetList.Draw( id );
                    WidgetDropdown.Draw( $"{id}/Widgets" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
    }
}
