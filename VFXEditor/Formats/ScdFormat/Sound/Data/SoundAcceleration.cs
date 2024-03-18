using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat.Sound.Data {
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

        public void Draw() {
            Version.Draw();
            Volume.Draw();
            UpTime.Draw();
            DownTime.Draw();
        }
    }

    public class SoundAcceleration {
        public readonly ParsedByte Version = new( "Version" );
        private byte Size = 0x10; // TODO: does this change with the size of acceleration?
        private readonly ParsedByte NumAcceleration = new( "Acceleration Count" );
        private readonly ParsedReserve Reserve1 = new( 1 + 4 * 3 );
        public List<SoundAccelerationInfo> Acceleration = [];

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
            Reserve1.Write( writer );

            Acceleration.ForEach( x => x.Write( writer ) );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Acceleration" );

            Version.Draw();
            NumAcceleration.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            for( var idx = 0; idx < Acceleration.Count; idx++ ) {
                if( ImGui.CollapsingHeader( $"Acceleration #{idx}", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    using var __ = ImRaii.PushId( idx );
                    using var indent = ImRaii.PushIndent();
                    Acceleration[idx].Draw();

                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                }
            }
        }
    }
}
