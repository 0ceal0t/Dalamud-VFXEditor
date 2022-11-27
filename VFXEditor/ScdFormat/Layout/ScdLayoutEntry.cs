using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class ScdLayoutEntry : ScdEntry, IScdSimpleUiBase {
        // Size = 0x80
        public readonly ParsedShort Unk1 = new( "Unknown 1" );
        public readonly ParsedInt Unk2 = new( "Unknown 2" );
        public readonly ParsedInt Unk3 = new( "Unknown 3" );
        public readonly ParsedInt Unk4 = new( "Unknown 4" );
        public readonly ParsedFloat4 Unk5 = new( "Unknown 5" );
        public readonly ParsedFloat4 Unk6 = new( "Unknown 6" );
        public readonly ParsedFloat4 Unk7 = new( "Unknown 7" );
        public readonly ParsedFloat4 Unk8 = new( "Unknown 8" );
        public readonly ParsedFloat4 Unk9 = new( "Unknown 9" );
        public readonly ParsedFloat4 Unk10 = new( "Unknown 10" );
        public readonly ParsedInt Unk11 = new( "Unknown 11" );
        public readonly ParsedShort Unk12 = new( "Unknown 12" );
        public readonly ParsedShort Unk13 = new( "Unknown 13" );
        public readonly ParsedInt Unk14 = new( "Unknown 14" );
        public readonly ParsedInt Unk15 = new( "Unknown 15" );

        private List<ParsedBase> Parsed;

        public ScdLayoutEntry( BinaryReader reader, int offset ) : base( reader, offset ) { }

        protected override void Read( BinaryReader reader ) {
            Parsed = new() {
                Unk1,
                Unk2,
                Unk3,
                Unk4,
                Unk5,
                Unk6,
                Unk7,
                Unk8,
                Unk9,
                Unk10,
                Unk11,
                Unk12,
                Unk13,
                Unk14,
                Unk15
            };

            reader.ReadInt16(); // size
            Parsed.ForEach( x => x.Read( reader ) );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( short )0x80 );
            Parsed.ForEach( x => x.Write( writer ) );
        }

        public void Draw( string id ) {
            Parsed.ForEach( x => x.Draw( id, CommandManager.Scd ) );
        }
    }
}
