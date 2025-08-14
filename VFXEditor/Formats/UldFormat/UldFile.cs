using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using Lumina.Extensions;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VfxEditor.UldFormat.Component;
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

        private readonly UldListHeader TextureList;
        public readonly List<UldTexture> Textures = [];

        private readonly UldListHeader PartList;
        public readonly List<UldPartList> Parts = [];

        private readonly UldListHeader ComponentList;
        public readonly List<UldComponent> Components = [];

        private readonly UldListHeader TimelineList;
        public readonly List<UldTimeline> Timelines = [];

        private readonly UldListHeader WidgetList;
        public readonly List<UldWidget> Widgets = [];

        public readonly CommandSplitView<UldTexture> TextureSplitView;
        public readonly CommandSplitView<UldPartList> PartsSplitView;
        public readonly CommandDropdown<UldComponent> ComponentDropdown;
        public readonly CommandDropdown<UldTimeline> TimelineDropdown;
        public readonly CommandDropdown<UldWidget> WidgetDropdown;

        public UldFile( BinaryReader reader, bool verify ) : base() {
            var pos = reader.BaseStream.Position;
            Header = new( reader ); // uldh 0100

            var offsetsPosition = reader.BaseStream.Position;
            OffsetsHeader = new( reader ); // atkh 0100

            // ==== ASSETS ======
            if( OffsetsHeader.AssetOffset > 0 ) {
                reader.Seek( offsetsPosition + OffsetsHeader.AssetOffset );
                TextureList = new( reader );
                for( var i = 0; i < TextureList.ElementCount; i++ ) Textures.Add( new( reader, TextureList.Version[3] ) );
            }
            else TextureList = new( "ashd", "0101" );

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
                for( var i = 0; i < ComponentList.ElementCount; i++ ) Components.Add( new( reader, Components ) );
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
                for( var i = 0; i < WidgetList.ElementCount; i++ ) Widgets.Add( new( reader, Components ) );
            }
            else WidgetList = new( "wdhd", "0100" );

            // Init node data now that components are all set up
            Components.ForEach( x => x.Nodes.ForEach( n => n.InitData( reader ) ) );
            Widgets.ForEach( x => x.Nodes.ForEach( n => n.InitData( reader ) ) );

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes() );

            TextureSplitView = new( "Texture", Textures, true,
                ( UldTexture item, int idx ) => item.GetText(), () => new( UldWorkspaceItem.GetNextId( Textures ) ) );

            PartsSplitView = new( "Part List", Parts, true,
                ( UldPartList item, int idx ) => item.GetText(), () => new( UldWorkspaceItem.GetNextId( Parts ) ) );

            ComponentDropdown = new( "Component", Components,
                ( UldComponent item, int idx ) => item.GetText(), () => new( UldWorkspaceItem.GetNextId( Components ), Components ) );

            TimelineDropdown = new( "Timeline", Timelines,
                ( UldTimeline item, int idx ) => item.GetText(), () => new( UldWorkspaceItem.GetNextId( Timelines ) ) );

            WidgetDropdown = new( "Widget", Widgets,
                ( UldWidget item, int idx ) => item.GetText(), () => new( UldWorkspaceItem.GetNextId( Widgets ), Components ) );
        }

        public override void Write( BinaryWriter writer ) {
            var pos = writer.BaseStream.Position;
            Header.Write( writer, out var headerUpdatePosition );

            var offsetsPosition = writer.BaseStream.Position;
            OffsetsHeader.Write( writer, out var offsetsUpdatePosition );

            // ====== ASSETS ========
            var assetOffset = TextureList.Write( writer, Textures, offsetsPosition );
            Textures.ForEach( x => x.Write( writer, TextureList.Version[3] ) );

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

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            if( UiUtils.BeginTabItem<UldTexture>( "Textures" ) ) {
                TextureList.Draw();
                TextureSplitView.Draw();
                ImGui.EndTabItem();
            }
            if( UiUtils.BeginTabItem<UldPartList>( "Part Lists" ) ) {
                PartList.Draw();
                PartsSplitView.Draw();
                ImGui.EndTabItem();
            }
            if( UiUtils.BeginTabItem<UldComponent>( "Components" ) ) {
                ComponentList.Draw();
                ComponentDropdown.Draw();
                ImGui.EndTabItem();
            }
            if( UiUtils.BeginTabItem<UldTimeline>( "Timelines" ) ) {
                TimelineList.Draw();
                TimelineDropdown.Draw();
                ImGui.EndTabItem();
            }
            if( UiUtils.BeginTabItem<UldWidget>( "Widgets" ) ) {
                WidgetList.Draw();
                WidgetDropdown.Draw();
                ImGui.EndTabItem();
            }
        }

        // ========== WORKSPACE ==========

        public Dictionary<string, string> GetRenamingMap() {
            Dictionary<string, string> ret = [];
            Textures.ForEach( x => IWorkspaceUiItem.GetRenamingMap( x, ret ) );
            Parts.ForEach( x => IWorkspaceUiItem.GetRenamingMap( x, ret ) );
            Components.ForEach( x => IWorkspaceUiItem.GetRenamingMap( x, ret ) );
            Timelines.ForEach( x => IWorkspaceUiItem.GetRenamingMap( x, ret ) );
            Widgets.ForEach( x => IWorkspaceUiItem.GetRenamingMap( x, ret ) );
            return ret;
        }

        public void ReadRenamingMap( Dictionary<string, string> renamingMap ) {
            Textures.ForEach( x => IWorkspaceUiItem.ReadRenamingMap( x, renamingMap ) );
            Parts.ForEach( x => IWorkspaceUiItem.ReadRenamingMap( x, renamingMap ) );
            Components.ForEach( x => IWorkspaceUiItem.ReadRenamingMap( x, renamingMap ) );
            Timelines.ForEach( x => IWorkspaceUiItem.ReadRenamingMap( x, renamingMap ) );
            Widgets.ForEach( x => IWorkspaceUiItem.ReadRenamingMap( x, renamingMap ) );
        }
    }
}
