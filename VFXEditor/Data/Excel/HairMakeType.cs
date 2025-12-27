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
        public readonly ExcelPage ExcelPage => page;
        public readonly uint RowOffset => offset;

        // Properties

        public const uint HairLength = 100;
        public const uint FacepaintLength = 50;

        public readonly uint HairStartIndex => page.ReadColumn<uint>( 66, offset );
        public readonly uint FacepaintStartIndex => page.ReadColumn<uint>( 82, offset );

        public readonly List<RowRef<CharaMakeCustomize>> HairStyles => GetRange( page, HairStartIndex, HairLength ).ToList();
        public readonly List<RowRef<CharaMakeCustomize>> Facepaints => GetRange( page, FacepaintStartIndex, FacepaintLength ).ToList();

        private static IEnumerable<RowRef<CharaMakeCustomize>> GetRange( ExcelPage page, uint start, uint length ) {
            for( var i = start; i < start + length; i++ )
                yield return new RowRef<CharaMakeCustomize>( page.Module, i, page.Language );
        }

        // Build sheet

        static HairMakeType IExcelRow<HairMakeType>.Create( ExcelPage page, uint offset, uint row ) => new( page, offset, row );
    }
}