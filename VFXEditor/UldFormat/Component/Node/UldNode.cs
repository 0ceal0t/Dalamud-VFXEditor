using Dalamud.Logging;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Parsing;
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
        Visible = 0x01,
        Enabled = 0x02,
        Clip = 0x04,
        Fill = 0x08,
        AnchorTop = 0x10,
        AnchorBottom = 0x20,
        AnchorLeft = 0x40,
        AnchorRight = 0x80
    }

    public class UldNode : UldWorkspaceItem {
        private readonly List<UldComponent> Components;
        private readonly UldWorkspaceItem Parent;

        public readonly ParsedInt ParentId = new( "Parent Id" );
        public readonly ParsedInt NextSiblingId = new( "Next Sibling Id" );
        public readonly ParsedInt PrevSiblingId = new( "Prev Sibling Id" );
        public readonly ParsedInt ChildNodeId = new( "Child Node Id" );

        public bool IsComponentNode = false;
        public readonly ParsedEnum<NodeType> Type = new( "Type" );
        public readonly ParsedInt ComponentTypeId = new( "Component Id" );
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
        public readonly ParsedAngle Rotation = new( "Rotation" );
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

        // need to wait until all components are initialized, so store this until then
        private readonly long DelayedPosition;
        private readonly int DelayedSize;
        private readonly int DelayedNodeType;

        public UldNode( List<UldComponent> components, UldWorkspaceItem parent ) {
            Parent = parent;
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

        public UldNode( BinaryReader reader, List<UldComponent> components, UldWorkspaceItem parent ) : this( components, parent ) {
            var pos = reader.BaseStream.Position;

            Id.Read( reader );
            ParentId.Read( reader );
            NextSiblingId.Read( reader );
            PrevSiblingId.Read( reader );
            ChildNodeId.Read( reader );

            var nodeType = reader.ReadInt32();
            var size = reader.ReadUInt16();
            // TODO: what if offset <= 88

            if( nodeType > 1000 ) {
                IsComponentNode = true;
                ComponentTypeId.Value = nodeType;
            }
            else {
                Type.Value = ( NodeType )nodeType;
            }

            Parsed.ForEach( x => x.Read( reader ) );

            DelayedPosition = reader.BaseStream.Position;
            DelayedSize = ( int )( pos + size - reader.BaseStream.Position ) - 12;
            DelayedNodeType = nodeType;

            reader.BaseStream.Position = pos + size;
        }

        // Needs to be initialized later since it depends on component list being filled
        public void InitData( BinaryReader reader ) {
            reader.BaseStream.Position = DelayedPosition;

            UpdateData();
            if( Data == null && DelayedNodeType > 1 ) {
                PluginLog.Log( $"Unknown node type {DelayedNodeType} / {DelayedSize} @ {reader.BaseStream.Position:X8}" );
            }
            if( Data is BaseNodeData custom ) custom.Read( reader, DelayedSize );
            else Data?.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            var pos = writer.BaseStream.Position;

            Id.Write( writer );
            ParentId.Write( writer );
            NextSiblingId.Write( writer );
            PrevSiblingId.Write( writer );
            ChildNodeId.Write( writer );

            if( IsComponentNode ) ComponentTypeId.Write( writer );
            else Type.Write( writer );

            var savePos = writer.BaseStream.Position;
            writer.Write( ( ushort )0 );

            Parsed.ForEach( x => x.Write( writer ) );
            Data?.Write( writer );

            var finalPos = writer.BaseStream.Position;
            var size = finalPos - pos;
            writer.BaseStream.Position = savePos;
            writer.Write( ( ushort )size );
            writer.BaseStream.Position = finalPos;
        }

        public void UpdateData() {
            if( IsComponentNode ) {
                var component = Components.Where( x => x.Id.Value == ComponentTypeId.Value ).FirstOrDefault();
                if( component == null ) Data = null;
                else {
                    Data = component.Type.Value switch {
                        //ComponentType.Custom => new CustomNodeData(),
                        ComponentType.Button => new ButtonNodeData(),
                        ComponentType.Window => new WindowNodeData(),
                        ComponentType.CheckBox => new CheckboxNodeData(),
                        ComponentType.RadioButton => new RadioButtonNodeData(),
                        ComponentType.Gauge => new GaugeNodeData(),
                        ComponentType.Slider => new SliderNodeData(),
                        ComponentType.TextInput => new TextInputNodeData(),
                        ComponentType.NumericInput => new NumericInputNodeData(),
                        ComponentType.List => new ListNodeData(),
                        ComponentType.Tabbed => new TabbedNodeData(),
                        ComponentType.ListItem => new ListItemNodeData(),
                        ComponentType.NineGridText => new NineGridTextNodeData(),
                        _ => new UldNodeComponentData()
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

        public override void Draw() {
            DrawRename();
            Id.Draw( CommandManager.Uld );

            if( ImGui.Checkbox( "Is Component Node", ref IsComponentNode ) ) CommandManager.Uld.Add( new UldNodeDataCommand( this, true ) );

            if( IsComponentNode ) {
                ComponentTypeId.Draw( CommandManager.Uld );
                ImGui.SameLine();
                if( ImGui.SmallButton( "Update" ) ) CommandManager.Uld.Add( new UldNodeDataCommand( this ) );
            }
            else Type.Draw( CommandManager.Uld );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters();
            DrawData();
        }

        private void DrawParameters() {
            using var tabItem = ImRaii.TabItem( "Parameters" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Parameters" );
            using var child = ImRaii.Child( "Child" );

            ParentId.Draw( CommandManager.Uld );
            NextSiblingId.Draw( CommandManager.Uld );
            PrevSiblingId.Draw( CommandManager.Uld );
            ChildNodeId.Draw( CommandManager.Uld );

            Parsed.ForEach( x => x.Draw( CommandManager.Uld ) );
        }

        private void DrawData() {
            if( Data == null ) return;

            using var tabItem = ImRaii.TabItem( "Data" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Data" );
            using var child = ImRaii.Child( "Child" );

            Data.Draw();
        }

        public override string GetDefaultText() {
            var suffix = IsComponentNode ? ComponentTypeId.Value.ToString() : Type.Value.ToString();
            return $"Node {GetIdx()} ({suffix})";
        }

        public override string GetWorkspaceId() => $"{Parent.GetWorkspaceId()}/Node{GetIdx()}";
    }
}
