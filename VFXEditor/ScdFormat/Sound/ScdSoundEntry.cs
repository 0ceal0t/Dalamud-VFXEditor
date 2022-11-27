using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

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
        FixedVolume = 0x0004,
        FixedPosition = 0x0008,

        Music = 0x0020,
        BypassPLIIz = 0x0040,
        UseExternalAttr = 0x0080,
        ExistRoutingSetting = 0x0100,
        MusicSurround = 0x0200,
        BusDucking = 0x0400,
        Acceleration = 0x0800,
        DynamixEnd = 0x1000,
        ExtraDesc = 0x2000,
        DynamixPlus = 0x4000,
        Atomosgear = 0x8000,
    }

    public class ScdSoundEntry : ScdEntry, IScdSimpleUiBase {
        public byte TrackCount = 0;
        public readonly ParsedByte BusNumber = new( "Bus Number" );
        public readonly ParsedByte Priority = new( "Priority" );
        public readonly ParsedEnum<SoundType> Type = new( "Type", size:1 );
        public readonly ParsedEnum<SoundAttribute> Attributes = new( "Attributes" );
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedShort LocalNumber = new( "Local Number" ); // TODO: ushort
        public readonly ParsedByte UserId = new( "User Id" );
        public readonly ParsedByte PlayHistory = new( "Play History" ); // TODO: sbyte

        private List<ParsedBase> Parsed;

        public readonly SoundRoutingInfo RoutingInfo = new(); // include sendInfos, soundEffectParam
        public SoundBusDucking BusDucking = new();
        public SoundAcceleration Acceleration = new();
        public SoundAtomos Atomos = new();
        public SoundExtra Extra = new();
        public SoundRandomTracks RandomTracks = new(); // pass track #, type
        // Includes Cycle ^
        public SoundTracks Tracks = new(); // pass track #

        private bool RoutingEnabled => Attributes.Value.HasFlag( SoundAttribute.ExistRoutingSetting );
        private bool BusDuckingEnabled => Attributes.Value.HasFlag( SoundAttribute.BusDucking );
        private bool AccelerationEnabled => Attributes.Value.HasFlag( SoundAttribute.Acceleration );
        private bool AtomosEnabled => Attributes.Value.HasFlag( SoundAttribute.Atomosgear );
        private bool ExtraEnabled => Attributes.Value.HasFlag( SoundAttribute.ExtraDesc );
        private bool RandomTracksEnabled => Type.Value == SoundType.Random || Type.Value == SoundType.Cycle || Type.Value == SoundType.GroupRandom;

        public ScdSoundEntry( BinaryReader reader, int offset ) : base( reader, offset ) { }

        protected override void Read( BinaryReader reader ) {
            Parsed = new() {
                BusNumber,
                Priority,
                Type,
                Attributes,
                Volume,
                LocalNumber,
                UserId,
                PlayHistory
            };

            TrackCount = reader.ReadByte();
            Parsed.ForEach( x => x.Read( reader ) );

            if( RoutingEnabled ) RoutingInfo.Read( reader );
            if( BusDuckingEnabled ) BusDucking.Read( reader );
            if( AccelerationEnabled ) Acceleration.Read( reader );
            if( AtomosEnabled ) Atomos.Read( reader );
            if( ExtraEnabled ) Extra.Read( reader );

            PluginLog.Log( $"{RoutingEnabled} {BusDuckingEnabled} {AccelerationEnabled} {AtomosEnabled} {ExtraEnabled}" );

            PluginLog.Log( $"reading.... {Type.Value} {TrackCount} {((int)(object)Attributes.Value):X8} {reader.BaseStream.Position:X8}" );

            if( RandomTracksEnabled ) RandomTracks.Read( reader, Type.Value, TrackCount );
            else Tracks.Read( reader, TrackCount );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( TrackCount );
            Parsed.ForEach( x => x.Write( writer ) );

            if( RoutingEnabled ) RoutingInfo.Write( writer );
            if( BusDuckingEnabled ) BusDucking.Write( writer );
            if( AccelerationEnabled ) Acceleration.Write( writer );
            if( AtomosEnabled ) Atomos.Write( writer );
            if( ExtraEnabled ) Extra.Write( writer );

            PluginLog.Log( $"writing.... {writer.BaseStream.Position:X8}" );

            if( RandomTracksEnabled ) RandomTracks.Write( writer, Type.Value );
            else Tracks.Write( writer );
        }

        public void Draw( string id ) {
            
        }
    }
}
