using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.SgbForamt.Layers;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbForamt.Scenes {
    public class SgbEntry : IUiItem {
        public readonly SgbFile File;
        public readonly ParsedString Id = new( "Id" );

        private bool UseTmb = false;
        private TmbFile EmbeddedTmb; // TODO: make a default file
        // TODO: missing tmb stuff

        private readonly List<SgbLayerGroup> LayerGroups = new();
        private readonly CommandDropdown<SgbLayerGroup> LayerGroupView;

        public SgbEntry( SgbFile file ) {
            File = file;
            LayerGroupView = new( "Layer Group", LayerGroups,
                ( SgbLayerGroup item, int idx ) => $"Layer Group {idx} ({item.Name.Value})", () => new SgbLayerGroup() );
        }

        public SgbEntry( SgbFile file, BinaryReader reader ) : this( file ) {
            var startPos = reader.BaseStream.Position;
            Id.Value = new string( reader.ReadChars( 4 ) ); // SCN1
            reader.ReadInt32(); // size, always 0x48

            var layerGroupOffset = reader.ReadInt32();
            var layerGroupCount = reader.ReadInt32();
            UseTmb = layerGroupOffset > 0x40; // if it's just 0x40, just the rest of the header

            var settings = reader.ReadInt32(); // 1C
            var layerSets = reader.ReadInt32(); // 20
            var sharedGroupTimelines = reader.ReadInt32(); // 24
            var layerGroupAssetPaths = reader.ReadInt32(); // 28
            var layerGroupAssetPathsCount = reader.ReadInt32(); // 2C
            var sharedGroupDoorSettings = reader.ReadInt32(); // 30
            var sharedGroupSettings = reader.ReadInt32(); // 34
            var sharedGroupRotationParams = reader.ReadInt32(); // 38
            var sharedGroupRandomTimelineParams = reader.ReadInt32(); // 3C
            var housingParams = reader.ReadInt32(); // 40
            var sharedGroupTimelineParams = reader.ReadInt32(); // 44

            reader.ReadBytes( 12 ); // Padding

            if( UseTmb ) {
                FileUtils.PadTo( reader, 16 );
                EmbeddedTmb = new( reader, File.Command, false );
            }

            for( var i = 0; i < layerGroupCount; i++ ) {
                reader.BaseStream.Seek( startPos + 8 + layerGroupOffset + ( i * 16 ), SeekOrigin.Begin ); // TODO: find an example with multiple layers
                LayerGroups.Add( new( reader ) );
            }

            // TODO: other stuff
        }

        public void Draw() {
            Id.Draw();
            if( ImGui.Checkbox( "Use Tmb", ref UseTmb ) ) {
                if( UseTmb && EmbeddedTmb == null ) {
                    // TODO: load default
                }
            }

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Layer Groups" ) ) {
                if( tab ) LayerGroupView.Draw();
            }

            if( UseTmb && EmbeddedTmb != null ) {
                using var tab = ImRaii.TabItem( "TMB" );
                if( tab ) {
                    using var _ = ImRaii.PushId( "Tmb" );
                    EmbeddedTmb.Draw();
                }
            }
        }
    }
}
