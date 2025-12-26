using Cyotek.Drawing.BitmapFont;
using Google.FlatBuffers;
using Lumina.Excel;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Utils;

namespace VfxEditor.Data.Excel {
    [Sheet( "HairMakeType" )]
    public struct HairMakeType( ExcelPage page, uint offset, uint row ) : IExcelRow<HairMakeType> {
        public readonly uint RowId => row;
        public ExcelPage ExcelPage => page;
        public uint RowOffset => offset;

        // Properties

        public const uint HairLength = 100;
        public const uint FacepaintLength = 50;

        public uint HairStartIndex { get; set; } // 66
        public uint FacepaintStartIndex { get; set; } // 82

        public List<RowRef<CharaMakeCustomize>> HairStyles { get; set; }
        public List<RowRef<CharaMakeCustomize>> Facepaints { get; set; }

        // Build sheet

        static HairMakeType IExcelRow<HairMakeType>.Create( ExcelPage page, uint offset, uint row ) => new( page, offset, row );

        /*

        static HairMakeType IExcelRow<HairMakeType>.Create( ExcelPage page, uint offset, uint row ) {
            var hairStartIndex = page.ReadColumn<uint>( 66, offset );
            var facePaintStartIndex = page.ReadColumn<uint>( 82, offset );
            return new HairMakeType( row ) {
                HairStartIndex = hairStartIndex,
                FacepaintStartIndex = facePaintStartIndex,
                HairStyles = GetRange( page, hairStartIndex, HairLength ).ToList(),
                Facepaints = GetRange( page, facePaintStartIndex, FacepaintLength ).ToList()
            };
        }

        private static IEnumerable<RowRef<CharaMakeCustomize>> GetRange( ExcelPage page, uint start, uint length ) {
            for( var i = start; i < start + length; i++ )
                yield return new RowRef<CharaMakeCustomize>( page.Module, i, page.Language );
        }

        */
    }
}