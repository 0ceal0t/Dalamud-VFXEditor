using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.IO;
using VfxEditor.Formats.ScdFormat.Sound.Data;
using VfxEditor.Parsing;
using VfxEditor.ScdFormat.Sound.Data;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.ScdFormat {
    public enum SoundType {
        Invalid = 0,
        Normal = 1,
        Random,
        Stereo,
        Cycle,
        Order,
        FourChannelSurround,
        Engine,
        Dialog,
        FixedPosition = 10,
        DynamixStream,
        GroupRandom,
        GroupOrder,
        Atomosgear,
        ConditionalJump,
        Empty,
        MidiMusic = 128
    }

    [Flags]
    public enum SoundAttribute {
        Invalid = 0,
        Loop = 0x0001,
        Reverb = 0x0002,
        Fixed_Volume = 0x0004,
        Fixed_Position = 0x0008,
        Music = 0x0020,
        Bypass_PLIIz = 0x0040,
        Use_External_Attr = 0x0080,
        Exist_Routing_Setting = 0x0100,
        Music_Surround = 0x0200,
        Bus_Ducking = 0x0400,
        Acceleration = 0x0800,
        Dynamix_End = 0x1000,
        Extra_Desc = 0x2000,
        Dynamix_Plus = 0x4000,
        Atomosgear = 0x8000,
    }

    public class ScdSoundEntry : ScdEntry, IUiItem {
        public readonly ParsedByte BusNumber = new( "Bus Number" );
        public readonly ParsedByte Priority = new( "Priority" );
        public readonly ParsedEnum<SoundType> Type = new( "Type", size: 1 );
        public readonly ParsedFlag<SoundAttribute> Attributes = new( "Attributes" );
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedShort LocalNumber = new( "Local Number" ); // TODO: ushort
        public readonly ParsedByte UserId = new( "User Id" );
        public readonly ParsedSByte PlayHistory = new( "Play History" );

        public readonly SoundRoutingInfo RoutingInfo = new(); // include sendInfos, soundEffectParam
        public SoundBusDucking BusDucking = new();
        public SoundAcceleration Acceleration = new();
        public SoundAtomos Atomos = new();
        public SoundExtra Extra = new();
        public SoundBypass BypassPLIIz = new();
        public SoundEmptyLoop EmptyLoop = new();

        public SoundRandomTracks RandomTracks = new(); // Includes Cycle
        public SoundTracks Tracks = new();

        private bool RoutingEnabled => Attributes.Value.HasFlag( SoundAttribute.Exist_Routing_Setting );
        private bool BusDuckingEnabled => Attributes.Value.HasFlag( SoundAttribute.Bus_Ducking );
        private bool AccelerationEnabled => Attributes.Value.HasFlag( SoundAttribute.Acceleration );
        private bool AtomosEnabled => Attributes.Value.HasFlag( SoundAttribute.Atomosgear );
        private bool ExtraEnabled => Attributes.Value.HasFlag( SoundAttribute.Extra_Desc );
        private bool BypassEnabled => Attributes.Value.HasFlag( SoundAttribute.Bypass_PLIIz );
        private bool RandomTracksEnabled => Type.Value == SoundType.Random || Type.Value == SoundType.Cycle || Type.Value == SoundType.GroupRandom || Type.Value == SoundType.GroupOrder;
        private bool IsEmptyLoop => Type.Value == SoundType.Empty && Attributes.Value.HasFlag( SoundAttribute.Loop );

        public readonly ScdLayoutEntry Layout;

        public ScdSoundEntry() : this( new() ) { }

        public ScdSoundEntry( ScdLayoutEntry layout ) {
            Layout = layout;
        }

        public override void Read( BinaryReader reader ) {
            var trackCount = reader.ReadByte();
            BusNumber.Read( reader );
            Priority.Read( reader );
            Type.Read( reader );
            Attributes.Read( reader );
            Volume.Read( reader );
            LocalNumber.Read( reader );
            UserId.Read( reader );
            PlayHistory.Read( reader );

            if( RoutingEnabled ) RoutingInfo.Read( reader );
            if( BusDuckingEnabled ) BusDucking.Read( reader );
            if( AccelerationEnabled ) Acceleration.Read( reader );
            if( AtomosEnabled ) Atomos.Read( reader );
            if( ExtraEnabled ) Extra.Read( reader );
            if( BypassEnabled ) BypassPLIIz.Read( reader );
            if( IsEmptyLoop ) EmptyLoop.Read( reader ); // kind of a guess

            if( RandomTracksEnabled ) RandomTracks.Read( reader, Type.Value, trackCount );
            else Tracks.Read( reader, trackCount );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( byte )( RandomTracksEnabled ? RandomTracks.Entries.Count : Tracks.Entries.Count ) );
            BusNumber.Write( writer );
            Priority.Write( writer );
            Type.Write( writer );
            Attributes.Write( writer );
            Volume.Write( writer );
            LocalNumber.Write( writer );
            UserId.Write( writer );
            PlayHistory.Write( writer );

            if( RoutingEnabled ) RoutingInfo.Write( writer );
            if( BusDuckingEnabled ) BusDucking.Write( writer );
            if( AccelerationEnabled ) Acceleration.Write( writer );
            if( AtomosEnabled ) Atomos.Write( writer );
            if( ExtraEnabled ) Extra.Write( writer );
            if( BypassEnabled ) BypassPLIIz.Write( writer );
            if( IsEmptyLoop ) EmptyLoop.Write( writer );

            if( RandomTracksEnabled ) RandomTracks.Write( writer, Type.Value );
            else Tracks.Write( writer );
        }

        public void Draw() {
            Volume.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var _ = ImRaii.PushId( "Sound" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "Entries" ) ) {
                if( RandomTracksEnabled ) RandomTracks.Draw( Type.Value );
                else Tracks.Draw();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Parameters" ) ) {
                using( var child = ImRaii.Child( "Child" ) ) {
                    Attributes.Draw();
                    BusNumber.Draw();
                    Priority.Draw();
                    Type.Draw();
                    LocalNumber.Draw();
                    UserId.Draw();
                    PlayHistory.Draw();
                }
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Layout" ) ) {
                using( var child = ImRaii.Child( "Child" ) ) {
                    Layout.Draw();
                }
                ImGui.EndTabItem();
            }
            if( RoutingEnabled && ImGui.BeginTabItem( "Routing" ) ) {
                using( var child = ImRaii.Child( "Child" ) ) {
                    RoutingInfo.Draw();
                }
                ImGui.EndTabItem();
            }
            if( BusDuckingEnabled && ImGui.BeginTabItem( "Bus Ducking" ) ) {
                using( var child = ImRaii.Child( "Child" ) ) {
                    BusDucking.Draw();
                }
                ImGui.EndTabItem();
            }
            if( AccelerationEnabled && ImGui.BeginTabItem( "Acceleration" ) ) {
                using( var child = ImRaii.Child( "Child" ) ) {
                    Acceleration.Draw();
                }
                ImGui.EndTabItem();
            }
            if( AtomosEnabled && ImGui.BeginTabItem( "Atomos" ) ) {
                using( var child = ImRaii.Child( "Child" ) ) {
                    Atomos.Draw();
                }
                ImGui.EndTabItem();
            }
            if( ExtraEnabled && ImGui.BeginTabItem( "Extra" ) ) {
                using( var child = ImRaii.Child( "Child" ) ) {
                    Extra.Draw();
                }
                ImGui.EndTabItem();
            }
            if( BypassEnabled && ImGui.BeginTabItem( "Bypass PLIIz" ) ) {
                using( var child = ImRaii.Child( "Child" ) ) {
                    BypassPLIIz.Draw();
                }
                ImGui.EndTabItem();
            }
            if( IsEmptyLoop && ImGui.BeginTabItem( "Empty Loop" ) ) {
                using( var child = ImRaii.Child( "Child" ) ) {
                    EmptyLoop.Draw();
                }
                ImGui.EndTabItem();
            }
        }
    }
}
