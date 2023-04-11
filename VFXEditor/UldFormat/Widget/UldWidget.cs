using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
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
 
    public class UldWidget {
        public readonly ParsedUInt Id = new( "Id" );
        public readonly ParsedEnum<AlignmentType> AlignmentType = new( "Alignment Type" );
        public readonly ParsedShort X = new( "X" );
        public readonly ParsedShort Y = new( "Y" );

        public readonly List<UldNode> Nodes = new();
        public readonly UldNodeSplitView NodeSplitView;

        public UldWidget( List<UldComponent> components ) {
            NodeSplitView = new( Nodes, components );
        }

        public UldWidget( BinaryReader reader, List<UldComponent> components ) : this( components ) {
            var pos = reader.BaseStream.Position;

            Id.Read( reader );
            AlignmentType.Read( reader );
            X.Read( reader );
            Y.Read( reader );
            var nodeCount = reader.ReadUInt16();
            var offset = reader.ReadUInt16();

            for( var i = 0; i < nodeCount; i++ ) Nodes.Add( new UldNode( reader, components ) );

            reader.BaseStream.Position = pos + offset;
        }

        public void Writer( BinaryWriter writer ) {

        }

        public void Draw( string id ) {
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
    }
}
