using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Interfaces;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxLife : AvfxOptional {
        public bool Enabled = true;
        // Life is kinda strange, can either be -1 (4 bytes = ffffffff) or Val + ValR + RanT

        public readonly AvfxFloat Value = new( "Value", "Val", value: -1 );
        public readonly AvfxFloat ValRandom = new( "Random Value", "ValR" );
        public readonly AvfxEnum<RandomType> ValRandomType = new( "Random Type", "Type" );

        private readonly List<AvfxBase> Parsed;
        private readonly List<IUiItem> Display;

        public AvfxLife() : base( "Life", locked: true ) {
            Parsed = [
                Value,
                ValRandom,
                ValRandomType
            ];

            Display = [
                Value,
                ValRandom,
                ValRandomType
            ];
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            if( size > 4 ) {
                Enabled = true;
                ReadNested( reader, Parsed, size );
                return;
            }
            Enabled = false;
        }

        public override void WriteContents( BinaryWriter writer ) {
            if( !Enabled ) {
                writer.Write( -1 );
                return;
            }
            WriteNested( writer, Parsed );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() {
            DrawItems( Display );
        }

        public override string GetDefaultText() => "Life";
    }
}
