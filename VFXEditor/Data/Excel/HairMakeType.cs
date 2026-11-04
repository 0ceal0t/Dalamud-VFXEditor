// From https://github.com/Etheirys/Brio/blob/main/Brio/Resources/Sheets/BrioHairMakeType.cs

using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Data.Excel;

[Sheet( "HairMakeType" )]
public struct HairMakeType( ExcelPage page, uint offset, uint row ) : IExcelRow<HairMakeType> {
    public const int StructSize = 452;
    public const int MaxParams = 106; // SubMenuParam count per struct
    public const int ParamOffset = 0xC;
    public const int SubMenuNumOffset = 438;
    public const int HairStyleStruct = 0; // Customize = 6
    public const int FacePaintStruct = 7; // Customize = 24

    public readonly ExcelPage ExcelPage => page;
    public readonly uint RowOffset => offset;
    public readonly uint RowId => row;

    public RowRef<Race> Race { get; private set; }
    public RowRef<Tribe> Tribe { get; private set; }
    public uint Gender { get; private set; }

    public RowRef<CharaMakeCustomize>[] HairStyles = [];
    public RowRef<CharaMakeCustomize>[] FacePaints = [];

    public static HairMakeType Create( ExcelPage page, uint offset, uint row ) {
        return new HairMakeType( page, offset, row ) {
            Race = new RowRef<Race>( page.Module, ( uint )page.ReadInt32( offset + 4292 ), page.Language ),
            Tribe = new RowRef<Tribe>( page.Module, ( uint )page.ReadInt32( offset + 4296 ), page.Language ),
            Gender = ( uint )page.ReadInt8( offset + 4300 ),
            HairStyles = ReadParams( page, offset, HairStyleStruct ),
            FacePaints = ReadParams( page, offset, FacePaintStruct ),
        };
    }

    private static RowRef<CharaMakeCustomize>[] ReadParams( ExcelPage page, uint offset, int structIdx ) {
        var structOffset = offset + ( uint )( structIdx * StructSize );
        var count = Math.Min( ( int )page.ReadUInt8( structOffset + SubMenuNumOffset ), MaxParams );
        var ret = new RowRef<CharaMakeCustomize>[count];
        for( var i = 0; i < count; i++ )
            ret[i] = new RowRef<CharaMakeCustomize>( page.Module, page.ReadUInt32( structOffset + ParamOffset + ( uint )( i * 4 ) ), page.Language );
        return ret;
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