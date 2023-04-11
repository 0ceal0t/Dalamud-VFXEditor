using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Timeline.Frames {
    public class UldFrame : ISimpleUiBase {
        public readonly ParsedUInt StartFrame = new( "Start Frame" );
        public readonly ParsedUInt EndFrame = new( "End Frame" );
        public readonly List<UldKeyGroup> KeyGroups = new();

        public readonly UldKeyGroupSplitView KeyGroupView;
        
        public UldFrame() {
            KeyGroupView = new( KeyGroups );
        }

        public UldFrame( BinaryReader reader ) : this() {
            var pos = reader.BaseStream.Position;

            StartFrame.Read( reader );
            EndFrame.Read( reader );
            var offset = reader.ReadUInt32();
            var count = reader.ReadUInt32();
            for( var i = 0; i < count; i++ ) KeyGroups.Add( new( reader ) );

            reader.BaseStream.Position = pos + offset;
        }

        public void Write( BinaryWriter writer ) {
        
        }

        public void Draw( string id ) {
            StartFrame.Draw( id, CommandManager.Uld );
            EndFrame.Draw( id, CommandManager.Uld );

            ImGui.BeginChild( id, new Vector2( -1, -1 ), true );
            KeyGroupView.Draw( $"{id}/Keys" );
            ImGui.EndChild();
        }
    }
}
