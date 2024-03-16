using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.UldFormat.Timeline.Frames {
    public class UldKeyframe : IUiItem {
        public readonly ParsedUInt Time = new( "Time" );
        public readonly ParsedInt Interpolation = new( "Interpolation", size: 1 );
        public readonly ParsedInt Unk1 = new( "Unknown", size: 1 );
        public readonly ParsedFloat Acceleration = new( "Acceleration" );
        public readonly ParsedFloat Deceleration = new( "Deceleration" );

        public readonly List<ParsedBase> Data = new();

        public UldKeyframe( KeyGroupType groupType ) {
            Data.AddRange( groupType switch {
                KeyGroupType.Float1 => new List<ParsedBase>() {
                    new ParsedFloat( "Value" )
                },
                KeyGroupType.Float2 => new List<ParsedBase>() {
                    new ParsedFloat2( "Value" )
                },
                KeyGroupType.Float3 => new List<ParsedBase>() {
                    new ParsedFloat3( "Value" )
                },
                KeyGroupType.SByte1 => new List<ParsedBase>() {
                    new ParsedSByte( "Value" ),
                    new ParsedReserve( 3 )
                },
                KeyGroupType.SByte2 => new List<ParsedBase>() {
                    new ParsedSByte( "Value 1" ), new ParsedSByte( "Value 2" ),
                    new ParsedReserve( 2 )
                },
                KeyGroupType.SByte3 => new List<ParsedBase>() {
                    new ParsedSByte( "Value 1" ), new ParsedSByte( "Value 2" ), new ParsedSByte( "Value 3" ),
                    new ParsedReserve( 1 )
                },
                KeyGroupType.Byte1 => new List<ParsedBase>() {
                    new ParsedInt( "Value", size: 1 ),
                    new ParsedReserve( 3 )
                },
                KeyGroupType.Byte2 => new List<ParsedBase>() {
                    new ParsedInt( "Value 1", size: 1 ), new ParsedInt( "Value 2", size: 1 ),
                    new ParsedReserve( 2 )
                },
                KeyGroupType.Byte3 => new List<ParsedBase>() {
                    new ParsedInt( "Value 1", size: 1 ), new ParsedInt( "Value 2", size: 1 ), new ParsedInt( "Value 3", size: 1 ),
                    new ParsedReserve( 1 )
                },
                KeyGroupType.Short1 => new List<ParsedBase>() {
                    new ParsedShort( "Value" ),
                    new ParsedReserve( 2 )
                },
                KeyGroupType.Short2 => new List<ParsedBase>() {
                    new ParsedShort2( "Value" ),
                    new ParsedReserve( 2 )
                },
                KeyGroupType.Short3 => new List<ParsedBase>() {
                    new ParsedShort3( "Value" ),
                    new ParsedReserve( 2 )
                },
                KeyGroupType.UShort1 => new List<ParsedBase>() {
                    new ParsedUInt( "Value", size: 2 ),
                    new ParsedReserve( 2 )
                },
                KeyGroupType.UShort2 => new List<ParsedBase>() {
                    new ParsedUInt( "Value 1", size: 2 ), new ParsedUInt( "Value 2", size: 2 ),
                    new ParsedReserve( 2 )
                },
                KeyGroupType.UShort3 => new List<ParsedBase>() {
                    new ParsedUInt( "Value 1", size: 2 ), new ParsedUInt( "Value 2", size: 2 ), new ParsedUInt( "Value 3", size: 2 ),
                    new ParsedReserve( 2 )
                },
                KeyGroupType.Int1 => new List<ParsedBase>() {
                    new ParsedInt( "Value 1" )
                },
                KeyGroupType.Int2 => new List<ParsedBase>() {
                    new ParsedInt( "Value 1" ), new ParsedInt( "Value 2" )
                },
                KeyGroupType.Int3 => new List<ParsedBase>() {
                    new ParsedInt( "Value 1" ), new ParsedInt( "Value 2" ), new ParsedInt( "Value 3" )
                },
                KeyGroupType.UInt1 => new List<ParsedBase>() {
                    new ParsedUInt( "Value" )
                },
                KeyGroupType.UInt2 => new List<ParsedBase>() {
                    new ParsedUInt( "Value 1" ), new ParsedUInt( "Value 2" )
                },
                KeyGroupType.UInt3 => new List<ParsedBase>() {
                    new ParsedUInt( "Value 1" ), new ParsedUInt( "Value 2" ), new ParsedUInt( "Value 3" )
                },
                KeyGroupType.Bool1 => new List<ParsedBase>() {
                    new ParsedByteBool( "Value" ),
                    new ParsedReserve( 3 )
                },
                KeyGroupType.Bool2 => new List<ParsedBase>() {
                    new ParsedByteBool( "Value 1" ), new ParsedByteBool( "Value 2" ),
                    new ParsedReserve( 2 )
                },
                KeyGroupType.Bool3 => new List<ParsedBase>() {
                    new ParsedByteBool( "Value 1" ), new ParsedByteBool( "Value 2" ), new ParsedByteBool( "Value 3" ),
                    new ParsedReserve( 1 )
                },
                KeyGroupType.Color => new List<ParsedBase>() {
                    new ParsedShort3( "Multiply Color" ),
                    new ParsedShort3( "Add Color" )
                },
                KeyGroupType.Label => new List<ParsedBase>() {
                    new ParsedUInt( "Label Id", size: 2 ), new ParsedInt( "Label Command", size: 1 ), new ParsedInt( "Jump Id", size: 1 )
                },
                _ => Array.Empty<ParsedBase>()
            } );
        }

        public UldKeyframe( BinaryReader reader, KeyGroupType groupType ) : this( groupType ) {
            var pos = reader.BaseStream.Position;

            Time.Read( reader );
            var size = reader.ReadUInt16();
            Interpolation.Read( reader );
            Unk1.Read( reader );
            Acceleration.Read( reader );
            Deceleration.Read( reader );

            Data.ForEach( x => x.Read( reader ) );

            reader.BaseStream.Position = pos + size;
        }

        public void Write( BinaryWriter writer ) {
            var pos = writer.BaseStream.Position;

            Time.Write( writer );

            var savePos = writer.BaseStream.Position;
            writer.Write( ( ushort )0 );

            Interpolation.Write( writer );
            Unk1.Write( writer );
            Acceleration.Write( writer );
            Deceleration.Write( writer );

            Data.ForEach( x => x.Write( writer ) );

            var finalPos = writer.BaseStream.Position;
            var size = finalPos - pos;
            writer.BaseStream.Position = savePos;
            writer.Write( ( ushort )size );
            writer.BaseStream.Position = finalPos;
        }

        public void Draw() {
            Time.Draw();
            Interpolation.Draw();
            Unk1.Draw();
            Acceleration.Draw();
            Deceleration.Draw();

            Data.ForEach( x => x.Draw() );
        }

        public string GetText() => $"Frame {Time.Value}";
    }
}
