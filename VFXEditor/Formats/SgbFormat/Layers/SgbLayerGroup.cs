using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbFormat.Layers {
    public class SgbLayerGroup : IUiItem {
        public readonly ParsedUInt Id = new( "Id" );
        public readonly ParsedString Name = new( "Name" );

        private readonly List<SgbLayer> Layers = new();
        private readonly UiSplitView<SgbLayer> LayerView;

        public SgbLayerGroup() {
            LayerView = new( "Layer", Layers, false, false );
        }

        public SgbLayerGroup( BinaryReader reader ) : this() {
            var startPos = reader.BaseStream.Position;
            Id.Read( reader );
            Name.Value = FileUtils.ReadStringOffset( startPos, reader );
            var layersOffsets = reader.ReadUInt32();
            var layerCount = reader.ReadUInt32();
            var endPos = reader.BaseStream.Position;

            foreach( var offset in FileUtils.ReadOffsets( layerCount, startPos + layersOffsets, reader ) ) {
                reader.BaseStream.Position = startPos + layersOffsets + offset;
                Layers.Add( new( reader ) );
            }

            reader.BaseStream.Position = endPos; // reset position
        }

        public void Draw() {
            Id.Draw();
            Name.Draw();
            ImGui.Separator();
            LayerView.Draw();
        }
    }
}
