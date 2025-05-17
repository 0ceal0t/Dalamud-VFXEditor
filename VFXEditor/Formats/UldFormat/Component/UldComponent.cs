using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Data;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VfxEditor.UldFormat.Component.Data;
using VfxEditor.UldFormat.Component.Node;

namespace VfxEditor.UldFormat.Component {
    public enum ComponentType : int {
        Base = 0x0,
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
        HoldButton = 0x18,
        Unknown_25 = 0x19,
    }

    public class UldComponent : UldWorkspaceItem, IItemWithData<UldGenericData> {
        public readonly ParsedByteBool IgnoreInput = new( "Ignore Input" );
        public readonly ParsedByteBool DragArrow = new( "Drag Arrow" );
        public readonly ParsedByteBool DropArrow = new( "Drop Arrow" );

        public readonly ParsedDataEnum<ComponentType, UldGenericData> Type;
        public UldGenericData Data;

        public readonly List<UldNode> Nodes = [];
        public readonly CommandSplitView<UldNode> NodeSplitView;

        public UldComponent( uint id, List<UldComponent> components ) : base( id ) {
            Type = new( this, "Type", size: 1 );

            NodeSplitView = new( "Node", Nodes, true,
                ( UldNode item, int idx ) => item.GetText(), () => new UldNode( GetNextId( Nodes, 1001 ), components, this, NodeSplitView ) );
        }

        public UldComponent( BinaryReader reader, List<UldComponent> components ) : this( 0, components ) {
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
                Dalamud.Log( $"Unknown component type {( int )Type.Value} / {pos + offset - reader.BaseStream.Position} @ {reader.BaseStream.Position:X8}" );
            }

            if( Data is BaseComponentData custom ) custom.Read( reader, offset - 16 );
            else Data?.Read( reader );

            reader.BaseStream.Position = pos + offset;

            for( var i = 0; i < nodeCount; i++ ) Nodes.Add( new UldNode( reader, components, this, NodeSplitView ) );
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
                ComponentType.Base => new BaseComponentData(), // ?
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

                ComponentType.HoldButton => new HoldButtonComponentData(),
                ComponentType.Unknown_25 => new Unknown25ComponentData(),
                _ => null
            };
        }

        public void SetData( UldGenericData data ) { Data = data; }

        public UldGenericData GetData() => Data;

        public override void Draw() {
            DrawRename();
            Id.Draw();
            Type.Draw();
            ImGui.TextDisabled( $"Nodes referencing this component: {NumNodesReferencing}" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters();
            DrawData();
            DrawNodes();
        }

        private void DrawParameters() {
            using var tabItem = ImRaii.TabItem( "Parameters" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Parameters" );
            using var child = ImRaii.Child( "Child" );

            IgnoreInput.Draw();
            DragArrow.Draw();
            DropArrow.Draw();
        }

        private void DrawData() {
            if( Data == null ) return;

            using var tabItem = ImRaii.TabItem( "Data" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Data" );
            using var child = ImRaii.Child( "Child" );

            Data.Draw();
        }

        private void DrawNodes() {
            using var tabItem = ImRaii.TabItem( "Nodes" );
            if( !tabItem ) return;

            NodeSplitView.Draw();
        }

        public override string GetDefaultText() => $"Component {GetIdx()} ({Type.Value})";

        public override string GetWorkspaceId() => $"Comp{GetIdx()}";

        public override void GetChildrenRename( Dictionary<string, string> renameDict ) {
            Nodes.ForEach( x => IWorkspaceUiItem.GetRenamingMap( x, renameDict ) );
        }

        public override void SetChildrenRename( Dictionary<string, string> renameDict ) {
            Nodes.ForEach( x => IWorkspaceUiItem.ReadRenamingMap( x, renameDict ) );
        }

        private int NumNodesReferencing =>
            Plugin.UldManager.File.Components.Select( c => c.Nodes.Where( x => x.IsComponentNode && x.ComponentTypeId.Value == Id.Value ).Count() ).Sum() +
            Plugin.UldManager.File.Widgets.Select( c => c.Nodes.Where( x => x.IsComponentNode && x.ComponentTypeId.Value == Id.Value ).Count() ).Sum();
    }
}
