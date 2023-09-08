using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.SklbFormat.Data {
    public class SklbOldData : SklbData {
        private readonly List<ParsedBase> Parsed;

        public SklbOldData( BinaryReader reader ) {
            reader.ReadInt16(); // layer offset, always 0x2E
            HavokOffset = reader.ReadInt16();

            Parsed = new() {
                Id,
                new ParsedShort2( "Parent 1"),
                new ParsedShort2( "Parent 2"),
                new ParsedShort2( "Parent 3"),
                new ParsedShort2( "Parent 4"),
                new ParsedShort( "LoD Bones 1" ),
                new ParsedShort( "LoD Bones 2" ),
                new ParsedShort( "LoD Bones 3" ),
                new ParsedShort( "Connect Bones 1" ),
                new ParsedShort( "Connect Bones 2" ),
                new ParsedShort( "Connect Bones 3" ),
                new ParsedShort( "Connect Bones 4" )
            };

            Parsed.ForEach( x => x.Read( reader ) );
        }

        public override long Write( BinaryWriter writer ) {
            writer.Write( ( short )0x2E );
            var havokOffset = writer.BaseStream.Position;
            writer.Write( ( short )0 ); // placeholder

            Parsed.ForEach( x => x.Write( writer ) );

            return havokOffset;
        }

        public override void Draw() {
            ImGui.TextDisabled( "Header Version: [OLD]" );

            Parsed.ForEach( x => x.Draw( CommandManager.Sklb ) );
        }
    }
}
