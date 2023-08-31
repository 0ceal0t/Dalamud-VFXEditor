using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat.Data {
    public class SklbNewData : SklbData {
        private readonly int DataSize;
        private readonly List<ParsedBase> Parsed;

        public SklbNewData( BinaryReader reader ) {
            DataSize = reader.ReadInt32();
            HavokOffset = reader.ReadInt32();

            Parsed = new() {
                new ParsedShort( "Bone Connect Index" ),
                new ParsedReserve( 2 ), // Padding
                Id
            };

            var numParents = DataSize == 0x30 ? 4 : 9; // 0x40 = 9
            for( var i = 0; i < numParents; i++ ) {
                Parsed.Add( new ParsedShort2( $"Parent {i + 1}" ) );
            }

            Parsed.ForEach( x => x.Read( reader ) );

            FileUtils.PadTo( reader, 16 );
        }

        public override long Write( BinaryWriter writer ) {
            writer.Write( DataSize );
            var havokOffset = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder

            Parsed.ForEach( x => x.Write( writer ) );

            FileUtils.PadTo( writer, 16 );

            return havokOffset;
        }

        public override void Draw() {
            ImGui.TextDisabled( "Header Version: [NEW]" );

            Parsed.ForEach( x => x.Draw( CommandManager.Sklb ) );
        }
    }
}
