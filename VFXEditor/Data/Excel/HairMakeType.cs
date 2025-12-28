// From https://github.com/Etheirys/Brio/blob/main/Brio/Resources/Sheets/BrioHairMakeType.cs

using Lumina.Excel;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Data.Excel;

[Sheet( "HairMakeType" )]
public struct HairMakeType( ExcelPage page, uint offset, uint row ) : IExcelRow<HairMakeType> {
    public const int EntryCount = 100;

    public readonly ExcelPage ExcelPage => page;
    public readonly uint RowOffset => offset;
    public readonly uint RowId => row;

    public RowRef<Race> Race { get; private set; }
    public RowRef<Tribe> Tribe { get; private set; }
    public uint Gender { get; private set; }


    public RowRef<CharaMakeCustomize>[] HairStyles = new RowRef<CharaMakeCustomize>[EntryCount];
    public RowRef<CharaMakeCustomize>[] FacePaints = new RowRef<CharaMakeCustomize>[EntryCount];

    public static HairMakeType Create( ExcelPage page, uint offset, uint row ) {
        var brioHairMakeType = new HairMakeType( page, offset, row ) {
            Race = new RowRef<Race>( page.Module, ( uint )page.ReadInt32( offset + 4292 ), page.Language ),
            Tribe = new RowRef<Tribe>( page.Module, ( uint )page.ReadInt32( offset + 4296 ), page.Language ),
            Gender = ( uint )page.ReadInt8( offset + 4300 )
        };

        for( int i = 0; i < EntryCount; i++ )
            brioHairMakeType.HairStyles[i] = new RowRef<CharaMakeCustomize>( page.Module, page.ReadUInt32( ( nuint )( offset + 0xC + ( i * 4 ) ) ), page.Language );

        for( int i = 0; i < EntryCount; i++ )
            brioHairMakeType.FacePaints[i] = new RowRef<CharaMakeCustomize>( page.Module, page.ReadUInt32( ( nuint )( offset + 0xBC0 + ( i * 4 ) ) ), page.Language );

        return brioHairMakeType;
    }

    public static IEnumerable<CharaMakeCustomize> GetHairStyles( uint gender, uint raceRowId, uint tribeRowId ) {
        var HairMakeType = Dalamud.DataManager.GetExcelSheet<HairMakeType>().Where( x => x.Gender == gender && x.Race.RowId == raceRowId && x.Tribe.RowId == tribeRowId );

        foreach( var item in HairMakeType ) {
            foreach( var item2 in item.HairStyles ) {
                if( item2.IsValid )
                    yield return item2.Value;
            }
        }
    }

    public static IEnumerable<CharaMakeCustomize> GetFacePaints( uint gender, uint raceRowId, uint tribeRowId ) {
        var FacePaints = Dalamud.DataManager.GetExcelSheet<HairMakeType>().Where( x => x.Gender == gender && x.Race.RowId == raceRowId && x.Tribe.RowId == tribeRowId );

        foreach( var item in FacePaints ) {
            foreach( var item2 in item.FacePaints ) {
                if( item2.IsValid )
                    yield return item2.Value;
            }
        }
    }
}