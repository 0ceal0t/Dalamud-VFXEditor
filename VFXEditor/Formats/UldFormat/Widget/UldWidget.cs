using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VfxEditor.UldFormat.Component;
using VfxEditor.UldFormat.Component.Node;

namespace VfxEditor.UldFormat.Widget {
    public enum AlignmentType : int {
        Unknown = 0xFF,
        TopLeft = 0x0,
        Top = 0x1,
        TopRight = 0x2,
        Left = 0x3,
        Center = 0x4,
        Right = 0x5,
        BottomLeft = 0x6,
        Bottom = 0x7,
        BottomRight = 0x8,
    }

    public class UldWidget : UldWorkspaceItem {
        public readonly ParsedEnum<AlignmentType> AlignmentType = new( "Alignment Type" );
        public readonly ParsedShort X = new( "X" );
        public readonly ParsedShort Y = new( "Y" );

        public readonly List<UldNode> Nodes = [];
        public readonly CommandSplitView<UldNode> NodeSplitView;

        public UldWidget( uint id, List<UldComponent> components ) : base( id ) {
            NodeSplitView = new( "Node", Nodes, true,
                ( item, idx ) => item.GetText(), () => new( GetNextId( Nodes ), components, this, NodeSplitView ) );
        }

        public UldWidget( BinaryReader reader, List<UldComponent> components ) : this( 0, components ) {
            var pos = reader.BaseStream.Position;

            Id.Read( reader );
            AlignmentType.Read( reader );
            X.Read( reader );
            Y.Read( reader );
            var nodeCount = reader.ReadUInt16();
            var size = reader.ReadUInt16();

            for( var i = 0; i < nodeCount; i++ ) Nodes.Add( new UldNode( reader, components, this, NodeSplitView ) );

            reader.BaseStream.Position = pos + size;
        }

        public void Write( BinaryWriter writer ) {
            var pos = writer.BaseStream.Position;

            Id.Write( writer );
            AlignmentType.Write( writer );
            X.Write( writer );
            Y.Write( writer );
            writer.Write( ( ushort )Nodes.Count );

            var savePos = writer.BaseStream.Position;
            writer.Write( ( ushort )0 );

            Nodes.ForEach( x => x.Write( writer ) );

            var finalPos = writer.BaseStream.Position;
            var size = finalPos - pos;
            writer.BaseStream.Position = savePos;
            writer.Write( ( ushort )size );
            writer.BaseStream.Position = finalPos;
        }

        public override void Draw() {
            DrawRename();
            Id.Draw();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters();
            DrawNodes();
        }

        private void DrawParameters() {
            using var tabItem = ImRaii.TabItem( "Parameters" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Parameters" );
            using var child = ImRaii.Child( "Child" );

            AlignmentType.Draw();
            X.Draw();
            Y.Draw();
        }

        private void DrawNodes() {
            using var tabItem = ImRaii.TabItem( "Nodes" );
            if( !tabItem ) return;

            NodeSplitView.Draw();
        }

        public override string GetDefaultText() => $"Widget {GetIdx()}";

        public override string GetWorkspaceId() => $"Widget{GetIdx()}";

        public override void GetChildrenRename( Dictionary<string, string> renameDict ) {
            Nodes.ForEach( x => IWorkspaceUiItem.GetRenamingMap( x, renameDict ) );
        }

        public override void SetChildrenRename( Dictionary<string, string> renameDict ) {
            Nodes.ForEach( x => IWorkspaceUiItem.ReadRenamingMap( x, renameDict ) );
        }
    }
}
