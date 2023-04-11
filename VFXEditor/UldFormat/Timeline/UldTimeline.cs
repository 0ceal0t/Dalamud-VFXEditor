using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.UldFormat.Timeline.Frames;

namespace VfxEditor.UldFormat.Timeline {
    public class UldTimeline {
        public readonly ParsedUInt Id = new( "Id" );

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
            var offset = reader.ReadUInt32(); // TODO: is this offset ok?
            var count1 = reader.ReadUInt16();
            var count2 = reader.ReadUInt16();
            for( var i = 0; i < count1; i++ ) Frames1.Add( new( reader ) );
            for( var i = 0; i < count2; i++ ) Frames2.Add( new( reader ) );

            reader.BaseStream.Position = pos + offset;
        }

        public void Write( BinaryWriter writer ) {

        }

        public void Draw( string id ) {
            Id.Draw( id, CommandManager.Uld );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.BeginTabBar( $"{id}/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                DrawFrames( "Frames 1", id, FramesView1 );
                DrawFrames( "Frames 2", id, FramesView2 );
                ImGui.EndTabBar();
            }
        }

        private void DrawFrames( string name, string id, UldFrameSplitView view ) {
            if( ImGui.BeginTabItem( $"{name}{id}" ) ) {
                var innerId = $"{id}/{name}";
                ImGui.BeginChild( innerId );
                view.Draw( innerId );
                ImGui.EndChild();
                ImGui.EndTabItem();
            }
        }
    }
}
