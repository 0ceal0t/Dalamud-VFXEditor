using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Parsing;
using VfxEditor.UldFormat.Component.Data;
using VfxEditor.UldFormat.Component.Node;

namespace VfxEditor.UldFormat.Component
{
    public enum ComponentType : int {
        Custom = 0x0,
        Button = 0x1,
        Window = 0x2,
        CheckBox = 0x3,
        RadioButton = 0x4,
        Gauge = 0x5,
        Slider = 0x6,
        TextInput = 0x7,
        NumericInput = 0x8,
        List = 0x9, //?
        DropDown = 0xA,
        Tabbed = 0xB,
        TreeList = 0xC,
        ScrollBar = 0xD,
        ListItem = 0xE,
        Icon = 0xF,
        IconWithText = 0x10,
        DragDrop = 0x11,
        LeveCard = 0x12,
        NineGridText = 0x13,
        Journal = 0x14,
        Multipurpose = 0x15,
        Map = 0x16,
        Preview = 0x17,
    }

    public class UldComponent {
        public readonly ParsedUInt Id = new( "Id" );
        public readonly ParsedByteBool IgnoreInput = new( "Ignore Input" );
        public readonly ParsedByteBool DragArrow = new( "Drag Arrow" );
        public readonly ParsedByteBool DropArrow = new( "Drop Arrow" );

        public readonly ParsedEnum<ComponentType> Type = new( "Type", size: 1 );
        public UldGenericData Data = null;

        public readonly List<UldNode> Nodes = new();
        public readonly UldNodeSplitView NodeSplitView;

        public UldComponent( List<UldComponent> components ) {
            NodeSplitView = new( Nodes, components );
            Type.ExtraCommandGenerator = () => {
                return new UldComponentDataCommand( this );
            };
        }

        public UldComponent( BinaryReader reader, List<UldComponent> components, List<DelayedNodeData> delayed ) : this( components ) {
            var pos = reader.BaseStream.Position;

            Id.Read( reader );
            IgnoreInput.Read( reader );
            DragArrow.Read( reader );
            DropArrow.Read( reader );
            Type.Read( reader );
            var nodeCount = reader.ReadUInt32();
            reader.ReadUInt16(); // size
            var offset = reader.ReadUInt16();

            UpdateData();
            if( Data == null ) {
                PluginLog.Log( $"Unknown component type {( int )Type.Value} / {pos + offset - reader.BaseStream.Position} @ {reader.BaseStream.Position:X8}" );
            }

            if( Data is CustomComponentData custom ) custom.Read( reader, offset - 16 );
            else Data?.Read( reader );

            reader.BaseStream.Position = pos + offset;

            for( var i = 0; i < nodeCount; i++ ) Nodes.Add( new UldNode( reader, components, delayed ) );
        }

        public void Write( BinaryWriter writer ) {
            var pos = writer.BaseStream.Position;

            Id.Write( writer );
            IgnoreInput.Write( writer );
            DragArrow.Write( writer );
            DropArrow.Write( writer );
            Type.Write( writer );
            writer.Write( Nodes.Count );

            var savePos = writer.BaseStream.Position;
            writer.Write( ( ushort )0 );
            writer.Write( ( ushort )0 );

            Data?.Write( writer );

            var nodePos = writer.BaseStream.Position;
            foreach( var node in Nodes ) node.Write( writer );

            var finalPos = writer.BaseStream.Position;
            var offset = nodePos - pos;
            var size = finalPos - pos;
            writer.BaseStream.Position = savePos;
            writer.Write( ( ushort )size );
            writer.Write( ( ushort )offset );
            writer.BaseStream.Position = finalPos;
        }

        public void UpdateData() {
            Data = Type.Value switch {
                ComponentType.Custom => new CustomComponentData(), // ?
                ComponentType.Button => new ButtonComponentData(),
                ComponentType.Window => new WindowComponentData(),
                ComponentType.CheckBox => new CheckboxComponentData(),
                ComponentType.RadioButton => new RadioButtonComponentData(),
                ComponentType.Gauge => new GaugeComponentData(),
                ComponentType.Slider => new SliderComponentData(),
                ComponentType.TextInput => new TextInputComponentData(),
                ComponentType.NumericInput => new NumericInputComponentData(),
                ComponentType.List => new ListComponentData(),
                ComponentType.DropDown => new DropDownComponentData(),
                ComponentType.Tabbed => new TabbedComponentData(),
                ComponentType.TreeList => new TreeListComponentData(),
                ComponentType.ScrollBar => new ScrollBarComponentData(),
                ComponentType.ListItem => new ListItemComponentData(),
                ComponentType.Icon => new IconComponentData(),
                ComponentType.IconWithText => new IconWithTextComponentData(),
                ComponentType.DragDrop => new DragDropComponentData(),
                ComponentType.LeveCard => new LeveCardComponentData(),
                ComponentType.NineGridText => new NineGridTextComponentData(),
                ComponentType.Journal => new JournalComponentData(),
                ComponentType.Multipurpose => new MultipurposeComponentData(),
                ComponentType.Map => new MapComponentData(),
                ComponentType.Preview => new PreviewComponentData(),
                _ => null
            };
        }

        public void Draw( string id ) {
            Id.Draw( id, CommandManager.Uld );
            ImGui.TextDisabled( "Component Ids must be greater than 1000" );
            Type.Draw( id, CommandManager.Uld );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.BeginTabBar( $"{id}/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( $"Parameters{id}" ) ) {
                    DrawParameters( $"{id}/Param" );
                    ImGui.EndTabItem();
                }
                if( Data != null && ImGui.BeginTabItem( $"Data{id}" ) ) {
                    DrawData( $"{id}/Data" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Nodes{id}" ) ) {
                    NodeSplitView.Draw( $"{id}/Nodes" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawParameters( string id ) {
            ImGui.BeginChild( id );
            IgnoreInput.Draw( id, CommandManager.Uld );
            DragArrow.Draw( id, CommandManager.Uld );
            DropArrow.Draw( id, CommandManager.Uld );
            ImGui.EndChild();
        }

        private void DrawData( string id ) {
            ImGui.BeginChild( id );
            Data.Draw( id );
            ImGui.EndChild();
        }
    }
}
