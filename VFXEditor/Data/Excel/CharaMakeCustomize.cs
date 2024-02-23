using Lumina.Data;
using Lumina.Excel;
using Lobby = Lumina.Excel.GeneratedSheets.Lobby;

namespace VfxEditor.Data.Excel {
    // https://github.com/ktisis-tools/Ktisis/blob/748922b02395ef9a700e3fe446f2fa2d6db0a63f/Ktisis/Data/Excel/CharaMakeCustomize.cs

    [Sheet( "CharaMakeCustomize" )]
    public class CharaMakeCustomize : ExcelRow {
        public byte FeatureId { get; private set; }
        public uint Icon { get; private set; }
        public ushort Data { get; private set; }
        public bool IsPurchasable { get; private set; }
        public LazyRow<Lobby> Hint { get; private set; } = null!;
        public byte FaceType { get; private set; }

        public override void PopulateData( RowParser parser, Lumina.GameData gameData, Language language ) {
            base.PopulateData( parser, gameData, language );

            FeatureId = parser.ReadColumn<byte>( 0 );
            Icon = parser.ReadColumn<uint>( 1 );
            Data = parser.ReadColumn<ushort>( 2 );
            IsPurchasable = parser.ReadColumn<bool>( 3 );
            Hint = new LazyRow<Lobby>( gameData, parser.ReadColumn<uint>( 4 ), language );
            FaceType = parser.ReadColumn<byte>( 6 );
        }
    }
}
