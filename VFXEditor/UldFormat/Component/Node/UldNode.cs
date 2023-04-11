using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.UldFormat.Component.Node.Data;
using VfxEditor.UldFormat.Component.Node.Data.Component;

namespace VfxEditor.UldFormat.Component.Node {
    public enum NodeType : int {
        Container = 1,
        Image = 2,
        Text = 3,
        NineGrid = 4,
        Counter = 5,
        Collision = 8,
    }

    [Flags]
    public enum NodeFlags {
        Visible = 0x80,
        Enabled = 0x40,
        Clip = 0x20,
        Fill = 0x10,
        AnchorTop = 0x08,
        AnchorBottom = 0x04,
        AnchorLeft = 0x02,
        AnchorRight = 0x01
    }

    public class UldNode : ISimpleUiBase {
        private readonly List<UldComponent> Components;

        public readonly ParsedUInt Id = new( "Id" );
        public readonly ParsedInt ParentId = new( "Parent Id" );
        public readonly ParsedInt NextSiblingId = new( "Next Sibling Id" );
        public readonly ParsedInt PrevSiblingId = new( "Prev Sibling Id" );
        public readonly ParsedInt ChildNodeId = new( "Child Node Id" );

        public bool IsComponentNode = false;
        public readonly ParsedEnum<NodeType> Type = new( "Type" ); // TODO: command
        public readonly ParsedInt ComponentTypeId = new( "Component Id" ); // TODO: change on update
        public UldGenericData Data = null;

        private readonly List<ParsedBase> Parsed;
        public readonly ParsedShort TabIndex = new( "Tab Index" );
        public readonly ParsedInt Unk1 = new( "Unknown 1" );
        public readonly ParsedInt Unk2 = new( "Unknown 2" );
        public readonly ParsedInt Unk3 = new( "Unknown 3" );
        public readonly ParsedInt Unk4 = new( "Unknown 4" );
        public readonly ParsedShort X = new( "X" );
        public readonly ParsedShort Y = new( "Y" );
        public readonly ParsedUInt W = new( "Width", size: 2 );
        public readonly ParsedUInt H = new( "Height", size: 2 );
        public readonly ParsedFloat Rotation = new( "Rotation" );
        public readonly ParsedFloat2 Scale = new( "Scale" );
        public readonly ParsedShort OriginX = new( "Origin X" );
        public readonly ParsedShort OriginY = new( "Origin Y" );
        public readonly ParsedUInt Priority = new( "Priority", size: 2 );
        public readonly ParsedFlag<NodeFlags> Flags = new( "Flags", size: 1 );
        public readonly ParsedInt Unk7 = new( "Unknown 7", size: 1 );
        public readonly ParsedShort MultiplyRed = new( "Multiply Red" );
        public readonly ParsedShort MultiplyGreen = new( "Multiply Green" );
        public readonly ParsedShort MultiplyBlue = new( "Multiply Blue" );
        public readonly ParsedShort AddRed = new( "Add Red" );
        public readonly ParsedShort AddGreen = new( "Add Green" );
        public readonly ParsedShort AddBlue = new( "Add Blue" );
        public readonly ParsedInt Alpha = new( "Alpha", size: 1 );
        public readonly ParsedInt ClipCount = new( "Clip Count", size: 1 );
        public readonly ParsedUInt TimelineId = new( "Timeline Id", size: 2 );

        public UldNode( List<UldComponent> components ) {
            Components = components;
            Type.ExtraCommandGenerator = () => {
                return new UldNodeDataCommand( this );
            };

            Parsed = new() {
                TabIndex,
                Unk1,
                Unk2,
                Unk3,
                Unk4,
                X,
                Y,
                W,
                H,
                Rotation,
                Scale,
                OriginX,
                OriginY,
                Priority,
                Flags,
                Unk7,
                MultiplyRed,
                MultiplyGreen,
                MultiplyBlue,
                AddRed,
                AddGreen,
                AddBlue,
                Alpha,
                ClipCount,
                TimelineId
            };
        }

        public UldNode( BinaryReader reader, List<UldComponent> components ) : this( components ) {
            var pos = reader.BaseStream.Position;

            Id.Read( reader );
            ParentId.Read( reader );
            NextSiblingId.Read( reader );
            PrevSiblingId.Read( reader );
            ChildNodeId.Read( reader );

            // Weirdness with node type
            var nodeType = reader.ReadInt32();
            var offset = reader.ReadUInt16();

            if( nodeType > 1000 ) {
                IsComponentNode = true;
                ComponentTypeId.Value = nodeType;
            }
            else {
                Type.Value = ( NodeType )nodeType;
            }

            Parsed.ForEach( x => x.Read( reader ) );

            // TODO: what if offset <= 88

            UpdateData();
            if( Data == null && nodeType > 1 ) {
                PluginLog.Log( $"Unknown node type {nodeType} / {pos + offset - reader.BaseStream.Position} @ {reader.BaseStream.Position:X8}" );
            }
            Data?.Read( reader );

            // TODO: what if there's some padding
            reader.BaseStream.Position = pos + offset;
        }

        public void Write( BinaryWriter writer ) {
            // TODO
        }

        public void UpdateData() {
            if( IsComponentNode ) {
                var component = Components.Where( x => x.Id.Value == ComponentTypeId.Value ).FirstOrDefault();
                if( component == null ) Data = null;
                else {
                    Data = component.Type.Value switch {
                        ComponentType.Button => new ButtonNodeData(),
                        ComponentType.Window => new WindowNodeData(),
                        ComponentType.CheckBox => new CheckboxNodeData(),
                        ComponentType.RadioButton => new RadioButtonNodeData(),
                        ComponentType.Gauge => new GaugeNodeData(),
                        ComponentType.Slider => new SliderNodeData(),
                        ComponentType.TextInput => new TextInputNodeData(),
                        ComponentType.NumericInput => new NumericInputNodeData(),
                        ComponentType.List => new ListNodeData(),
                        //ComponentType.DropDown => new DropDownNodeData(),
                        ComponentType.Tabbed => new TabbedNodeData(),
                        //ComponentType.TreeList => new TreeListNodeData(),
                        //ComponentType.ScrollBar => new ScollBarNodeData(),
                        ComponentType.ListItem => new ListItemNodeData(),
                        //ComponentType.Icon => new IconNodeData(),
                        ComponentType.IconWithText => new IconWithTextNodeData(),
                        //ComponentType.DragDrop => new DragDropNodeData(),
                        //ComponentType.LeveCard => new LeveCardNodeData(),
                        ComponentType.NineGridText => new NineGridTextNodeData(),
                        //ComponentType.Journal => new JournalNodeData(),
                        //ComponentType.Multipurpose => new MultipurposeNodeData(),
                        //Map => new MapNodeData(),
                        //ComponentType.Preview => new PreviewNodeData(),
                        _ => null
                    };
                }
            }
            else {
                Data = Type.Value switch {
                    NodeType.Image => new ImageNodeData(),
                    NodeType.Text => new TextNodeData(),
                    NodeType.NineGrid => new NineGridNodeData(),
                    NodeType.Counter => new CounterNodeData(),
                    NodeType.Collision => new CollisionNodeData(),
                    _ => null
                };
            }
        }

        public void Draw( string id ) {
            Id.Draw( id, CommandManager.Uld );
            // Update component state along with data
            if( ImGui.Checkbox( $"Component Node{id}", ref IsComponentNode ) ) CommandManager.Uld.Add( new UldNodeDataCommand( this, true ) );

            if( IsComponentNode ) {
                ComponentTypeId.Draw( id, CommandManager.Uld );
                ImGui.SameLine();
                if( ImGui.SmallButton( $"Update{id}" ) ) CommandManager.Uld.Add( new UldNodeDataCommand( this ) );
            }
            else Type.Draw( id, CommandManager.Uld );

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
                ImGui.EndTabBar();
            }
        }

        private void DrawParameters( string id ) {
            ImGui.BeginChild( id );
            ParentId.Draw( id, CommandManager.Uld );
            NextSiblingId.Draw( id, CommandManager.Uld );
            PrevSiblingId.Draw( id, CommandManager.Uld );
            ChildNodeId.Draw( id, CommandManager.Uld );

            Parsed.ForEach( x => x.Draw( id, CommandManager.Uld ) );
            ImGui.EndChild();
        }

        private void DrawData( string id ) {
            ImGui.BeginChild( id );
            Data.Draw( id );
            ImGui.EndChild();
        }
    }
}
