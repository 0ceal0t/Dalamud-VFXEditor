using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VfxEditor.UldFormat.Component;
using VfxEditor.UldFormat.Component.Node;

namespace VfxEditor.UldFormat.Widget {
    public enum AlignmentType : int {
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

        public readonly List<UldNode> Nodes = new();
        public readonly UldNodeSplitView NodeSplitView;

        public UldWidget( List<UldComponent> components ) {
            NodeSplitView = new( Nodes, components, this );
        }

        public UldWidget( BinaryReader reader, List<UldComponent> components, List<DelayedNodeData> delayed ) : this( components ) {
            var pos = reader.BaseStream.Position;

            Id.Read( reader );
            AlignmentType.Read( reader );
            X.Read( reader );
            Y.Read( reader );
            var nodeCount = reader.ReadUInt16();
            var size = reader.ReadUInt16();

            for( var i = 0; i < nodeCount; i++ ) Nodes.Add( new UldNode( reader, components, this, delayed ) );

            reader.BaseStream.Position = pos + size;
        }

        public void Write( BinaryWriter writer ) {
            var pos = writer.BaseStream.Position;

            Id.Write( writer );
            AlignmentType.Write( writer );
            X.Write( writer );
            Y.Write( writer );
            writer.Write( (ushort) Nodes.Count );

            var savePos = writer.BaseStream.Position;
            writer.Write( ( ushort )0 );

            Nodes.ForEach( x => x.Write( writer ) );

            var finalPos = writer.BaseStream.Position;
            var size = finalPos - pos;
            writer.BaseStream.Position = savePos;
            writer.Write( ( ushort )size );
            writer.BaseStream.Position = finalPos;
        }

        public override void Draw( string id ) {
            DrawRename( id );
            Id.Draw( id, CommandManager.Uld );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.BeginTabBar( $"{id}/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( $"Parameters{id}" ) ) {
                    DrawParameters( $"{id}/Param" );
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
            AlignmentType.Draw( id, CommandManager.Uld );
            X.Draw( id, CommandManager.Uld );
            Y.Draw( id, CommandManager.Uld );
            ImGui.EndChild();
        }

        public override string GetDefaultText() => $"Widget {GetIdx()}";

        public override string GetWorkspaceId() => $"Widget{GetIdx()}";

        public override void GetChildrenRename( Dictionary<string, string> renameDict ) {
            Nodes.ForEach( x => IWorkspaceUiItem.PopulateMeta( x, renameDict ) );
        }

        public override void SetChildrenRename( Dictionary<string, string> renameDict ) {
            Nodes.ForEach( x => IWorkspaceUiItem.ReadMeta( x, renameDict ) );
        }
    }
}
