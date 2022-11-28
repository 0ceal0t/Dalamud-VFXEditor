using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class SoundAccelerationInfo {
        public readonly ParsedByte Version = new( "Version" );
        private byte Size = 0x10;
        private readonly ParsedReserve Reserve1 = new( 2 );
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedInt UpTime = new( "Up Time" );
        public readonly ParsedInt DownTime = new( "Down Time" );

        public void Read( BinaryReader reader ) {
            Version.Read( reader );
            Size = reader.ReadByte();
            Reserve1.Read( reader );
            Volume.Read( reader );
            UpTime.Read( reader );
            DownTime.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Version.Write( writer );
            writer.Write( Size );
            Reserve1.Write( writer );
            Volume.Write( writer );
            UpTime.Write( writer );
            DownTime.Write( writer );
        }

        public void Draw( string parentId ) {
            Version.Draw( parentId, CommandManager.Scd );
            Volume.Draw( parentId, CommandManager.Scd );
            UpTime.Draw( parentId, CommandManager.Scd );
            DownTime.Draw( parentId, CommandManager.Scd );
        }
    }

    public class SoundAcceleration {
        public readonly ParsedByte Version = new( "Version" );
        private byte Size = 0x10; // TODO: does this change with the size of acceleration?
        private readonly ParsedByte NumAcceleration = new( "Acceleration Count" );
        private readonly ParsedReserve Reserve1 = new( 1 + 4 * 3 );
        public List<SoundAccelerationInfo> Acceleration = new();

        public void Read( BinaryReader reader ) {
            Version.Read( reader );
            Size = reader.ReadByte();
            NumAcceleration.Read( reader );
            Reserve1.Read( reader );

            for( var i = 0; i < 4; i++ ) {
                var newAcceleration = new SoundAccelerationInfo();
                newAcceleration.Read( reader );
                Acceleration.Add( newAcceleration );
            }
        }

        public void Write( BinaryWriter writer ) {
            Version.Write( writer );
            writer.Write( Size );
            NumAcceleration.Write( writer );
            Reserve1 .Write( writer );

            Acceleration.ForEach( x => x.Write( writer ) );
        }

        public void Draw( string parentId ) {
            Version.Draw( parentId, CommandManager.Scd );
            NumAcceleration.Draw( parentId, CommandManager.Scd );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            for( var idx = 0; idx < Acceleration.Count; idx++ ) {
                if( ImGui.CollapsingHeader( $"Acceleration #{idx}{parentId}", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    ImGui.Indent();
                    Acceleration[idx].Draw( $"{parentId}{idx}" );
                    ImGui.Unindent();

                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                }
            }
        }
    }
}
