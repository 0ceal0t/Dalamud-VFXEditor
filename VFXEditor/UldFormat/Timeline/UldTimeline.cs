using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.UldFormat.Timeline.Frames;

namespace VfxEditor.UldFormat.Timeline {
    public class UldTimeline : UldWorkspaceItem {
        public readonly List<UldFrame> Frames1 = new();
        public readonly List<UldFrame> Frames2 = new();

        public readonly UldFrameSplitView FramesView1;
        public readonly UldFrameSplitView FramesView2;

        public UldTimeline() {
            FramesView1 = new( Frames1 );
            FramesView2 = new( Frames2 );
        }

        public UldTimeline( BinaryReader reader ) : this() {
            var pos = reader.BaseStream.Position;

            Id.Read( reader );
            var size = reader.ReadUInt32();
            var count1 = reader.ReadUInt16();
            var count2 = reader.ReadUInt16();
            for( var i = 0; i < count1; i++ ) Frames1.Add( new( reader ) );
            for( var i = 0; i < count2; i++ ) Frames2.Add( new( reader ) );

            reader.BaseStream.Position = pos + size;
        }

        public void Write( BinaryWriter writer ) {
            var pos = writer.BaseStream.Position;

            Id.Write( writer );

            var savePos = writer.BaseStream.Position;
            writer.Write( ( uint )0 );

            writer.Write( ( ushort )Frames1.Count );
            writer.Write( ( ushort )Frames2.Count );
            Frames1.ForEach( x => x.Write( writer ) );
            Frames2.ForEach( x => x.Write( writer ) );

            var finalPos = writer.BaseStream.Position;
            var size = finalPos - pos;
            writer.BaseStream.Position = savePos;
            writer.Write( ( uint )size );
            writer.BaseStream.Position = finalPos;
        }

        public override void Draw( string id ) {
            DrawRename( id );
            Id.Draw( id, CommandManager.Uld );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.BeginTabBar( $"{id}/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                DrawFrames( "Frames 1", id, FramesView1 );
                DrawFrames( "Frames 2", id, FramesView2 );
                ImGui.EndTabBar();
            }
        }

        private static void DrawFrames( string name, string id, UldFrameSplitView view ) {
            if( ImGui.BeginTabItem( $"{name}{id}" ) ) {
                var innerId = $"{id}/{name}";
                ImGui.BeginChild( innerId );
                view.Draw( innerId );
                ImGui.EndChild();
                ImGui.EndTabItem();
            }
        }

        public override string GetDefaultText() => $"Timeline {GetIdx()}";

        public override string GetWorkspaceId() => $"Timeline{GetIdx()}";
    }
}
