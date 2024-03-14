using Dalamud.Game.ClientState.Objects.Enums;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using Lobby = Lumina.Excel.GeneratedSheets.Lobby;

namespace VfxEditor.Data.Excel {
    // https://github.com/ktisis-tools/Ktisis/blob/748922b02395ef9a700e3fe446f2fa2d6db0a63f/Ktisis/Data/Excel/CharaMakeType.cs

    public enum MenuType : byte {
        List = 0,
        Select = 1,
        Color = 2,
        Unknown1 = 3,
        SelectMulti = 4,
        Slider = 5
    }

    public struct Menu {
        public string Name;
        public byte Default;
        public MenuType Type;
        public byte Count;
        // LookAt
        // SubMenuMask
        public CustomizeIndex Index;
        public uint[] Params;
        public byte[] Graphics;

        public LazyRow<CharaMakeCustomize>[] Features;

        public readonly bool HasIcon => Type == MenuType.Select;
        public readonly bool IsFeature => HasIcon && Graphics[0] == 0;
    }

    [Sheet( "CharaMakeType" )]
    public class CharaMakeType : ExcelRow {
        private const int MENU_COUNT = 28;
        private const int VOICE_COUNT = 12;
        private const int GRAPHIC_COUNT = 10;

        public LazyRow<Race> Race { get; set; } = null!;
        public LazyRow<Tribe> Tribe { get; set; } = null!;
        public sbyte Gender { get; set; }

        public Menu[] Menus { get; set; } = new Menu[MENU_COUNT];
        public byte[] Voices { get; set; } = new byte[VOICE_COUNT];
        public int[] FacialFeatures { get; set; } = new int[7 * 8];

        public LazyRow<HairMakeType> FeatureMake { get; set; } = null!;

        public override void PopulateData( RowParser parser, Lumina.GameData gameData, Language language ) {
            base.PopulateData( parser, gameData, language );

            Race = new LazyRow<Race>( gameData, parser.ReadColumn<int>( 0 ), language );
            Tribe = new LazyRow<Tribe>( gameData, parser.ReadColumn<int>( 1 ), language );
            Gender = parser.ReadColumn<sbyte>( 2 );

            FeatureMake = new LazyRow<HairMakeType>( gameData, RowId, language );

            for( var i = 0; i < 7 * 8; i++ ) FacialFeatures[i] = parser.ReadColumn<int>( 3291 + i );

            for( var i = 0; i < MENU_COUNT; i++ ) {
                var ct = parser.ReadColumn<byte>( 3 + 3 * MENU_COUNT + i );
                var menu = new Menu() {
                    Name = new LazyRow<Lobby>( gameData, parser.ReadColumn<uint>( 3 + i ), language ).Value!.Text,
                    Default = parser.ReadColumn<byte>( 3 + 1 * MENU_COUNT + i ),
                    Type = ( MenuType )parser.ReadColumn<byte>( 3 + 2 * MENU_COUNT + i ),
                    Count = ct,
                    Index = ( CustomizeIndex )parser.ReadColumn<uint>( 3 + 6 * MENU_COUNT + i ),
                    Params = new uint[ct],
                    Graphics = new byte[GRAPHIC_COUNT]
                };

                if( menu.HasIcon || menu.Type == MenuType.List ) {
                    for( var p = 0; p < ct; p++ ) menu.Params[p] = parser.ReadColumn<uint>( 3 + ( 7 + p ) * MENU_COUNT + i );
                    for( var g = 0; g < GRAPHIC_COUNT; g++ ) menu.Graphics[g] = parser.ReadColumn<byte>( 3 + ( 107 + g ) * MENU_COUNT + i );
                }

                if( menu.IsFeature ) {
                    var feats = new LazyRow<CharaMakeCustomize>[ct];
                    for( var x = 0; x < ct; x++ ) feats[x] = new LazyRow<CharaMakeCustomize>( gameData, menu.Params[x] );
                    menu.Features = feats;
                }

                Menus[i] = menu;
            }
        }
    }
}
