using Lumina.Excel;
using Lumina.Excel.Sheets;
using VfxEditor.Utils;

namespace VfxEditor.Data.Excel {
    // https://github.com/ktisis-tools/Ktisis/blob/748922b02395ef9a700e3fe446f2fa2d6db0a63f/Ktisis/Data/Excel/CharaMakeCustomize.cs

    [Sheet( "CharaMakeCustomize" )]
    public struct CharaMakeCustomize( uint row ) : IExcelRow<CharaMakeCustomize> {
        public readonly uint RowId => row;

        public string Name { get; set; } = "";

        public byte FeatureId { get; set; }
        public uint Icon { get; set; }
        public ushort Data { get; set; }
        public bool IsPurchasable { get; set; }
        public RowRef<Lobby> Hint { get; set; }
        public byte FaceType { get; set; }

        static CharaMakeCustomize IExcelRow<CharaMakeCustomize>.Create( ExcelPage page, uint offset, uint row ) {
            return new CharaMakeCustomize( row ) {
                FeatureId = page.ReadColumn<byte>( 0, offset ),
                Icon = page.ReadColumn<uint>( 1, offset ),
                Data = page.ReadColumn<ushort>( 2, offset ),
                IsPurchasable = page.ReadColumn<bool>( 3, offset ),
                Hint = page.ReadRowRef<Lobby>( 4, offset ),
                FaceType = page.ReadColumn<byte>( 6, offset )
            };
        }
    }
}
