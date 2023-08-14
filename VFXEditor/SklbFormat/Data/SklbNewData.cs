using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.SklbFormat.Data {
    public class SklbNewData : SklbData {
        public readonly ParsedShort BoneConnectIndex = new( "Bone Connect Index" );
        public readonly ParsedShort2 Unk1 = new( "Unknown 1" );
        public readonly ParsedShort2 Unk2 = new( "Unknown 2" );

        private readonly List<ParsedBase> Parsed;

        public SklbNewData( BinaryReader reader ) {
            reader.ReadInt32(); // layer offset, always 0x30
            HavokOffset = reader.ReadInt32();

            Parsed = new() {
                new ParsedShort( "Bone Connect Index" ),
                new ParsedReserve( 2 ), // Padding
                Id,
                Parent1,
                Parent2,
                Parent3,
                Parent4,
                new ParsedShort2( "Unknown 1" ),
                new ParsedShort2( "Unknown 2" )
            };

            Parsed.ForEach( x => x.Read( reader ) );
        }

        public override long Write( BinaryWriter writer ) {
            writer.Write( 0x30 );
            var havokOffset = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder

            Parsed.ForEach( x => x.Write( writer ) );

            return havokOffset;
        }

        public override void Draw() {
            ImGui.TextDisabled( "Header Version: [NEW]" );

            Parsed.ForEach( x => x.Draw( CommandManager.Sklb ) );
        }
    }
}
