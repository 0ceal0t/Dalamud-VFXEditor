using Lumina.Data;
using Lumina.Excel;
using System.Collections.Generic;

namespace VfxEditor.Data.Excel {
    [Sheet( "HairMakeType" )]
    public class HairMakeType : ExcelRow {
        private const uint HAIR_LENGTH = 100;
        private const uint FACEPAINT_LENGTH = 50;

        public uint HairStartIndex { get; set; }
        public uint FacepaintStartIndex { get; set; }

        public List<LazyRow<CharaMakeCustomize>> HairStyles { get; set; } = [];
        public List<LazyRow<CharaMakeCustomize>> Facepaints { get; set; } = [];

        public override void PopulateData( RowParser parser, Lumina.GameData gameData, Language language ) {
            base.PopulateData( parser, gameData, language );

            HairStartIndex = parser.ReadColumn<uint>( 66 );
            FacepaintStartIndex = parser.ReadColumn<uint>( 82 );

            for( var i = HairStartIndex; i < HairStartIndex + HAIR_LENGTH; i++ ) HairStyles.Add( new LazyRow<CharaMakeCustomize>( gameData, i, language ) );
            for( var i = FacepaintStartIndex; i < FacepaintStartIndex + FACEPAINT_LENGTH; i++ ) Facepaints.Add( new LazyRow<CharaMakeCustomize>( gameData, i, language ) );
        }
    }
}
