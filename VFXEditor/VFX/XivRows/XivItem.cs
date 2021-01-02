using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor
{
    public struct XivItemIds
    {
        public int PrimaryId;
        public int PrimaryVar;
        public int SecondaryId;
        public int SecondaryVar;

        public XivItemIds( ulong modelDataRaw )
        {
            /*
             * Gear: [Id, Var, -, -] / [-,-,-,-]
             * Weapon: [Id, Var, Id, -] / [Id, Var, Id, -]
             */
            byte[] b = BitConverter.GetBytes( modelDataRaw );
            PrimaryId = BitConverter.ToInt16( b, 0 ); // primary key
            PrimaryVar = BitConverter.ToInt16( b, 2 ); // primary variant (weapon if != 0)
            SecondaryId = BitConverter.ToInt16( b, 4 ); // secondary key
            SecondaryVar = BitConverter.ToInt16( b, 6 ); // secondary variant
        }
    }

    public class XivItem
    {
        public bool HasSub;
        public XivItem SubItem = null;

        public string Name;
        public XivItemIds Ids;
        public XivItemIds SecondaryIds;
        public bool HasModel;
        public int Variant;

        public string rootPath;
        public string vfxPath;
        public string imcPath;

        public int RowId;

        public XivItem( Lumina.Excel.GeneratedSheets.Item item )
        {
            Name = item.Name.ToString();
            RowId = (int)item.RowId;

            Ids = new XivItemIds( item.ModelMain );
            SecondaryIds = new XivItemIds( item.ModelSub );
            HasModel = ( Ids.PrimaryId != 0 );
            HasSub = ( SecondaryIds.PrimaryId != 0 );

            if( HasSub )
            {
                var sItem = new Lumina.Excel.GeneratedSheets.Item();

                sItem.Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( Name + " / Offhand" ) );
                sItem.Icon = item.Icon;
                sItem.EquipRestriction = item.EquipRestriction;
                sItem.EquipSlotCategory = item.EquipSlotCategory;
                sItem.ItemSearchCategory = item.ItemSearchCategory;
                sItem.ItemSortCategory = item.ItemSortCategory;
                sItem.ClassJobCategory = item.ClassJobCategory;
                sItem.ItemUICategory = item.ItemUICategory;
                sItem.ModelMain = item.ModelSub;
                sItem.ModelSub = 0;
                SubItem = new XivItem( sItem );
            }

            rootPath = "chara/weapon/w" + Ids.PrimaryId.ToString().PadLeft( 4, '0' ) + "/obj/body/b" + Ids.PrimaryVar.ToString().PadLeft( 4, '0' ) + "/";
            vfxPath = rootPath + "vfx/eff/vw";
            Variant = Ids.SecondaryId;
        }

        public string GetImcPath()
        {
            return rootPath + "b" + Ids.PrimaryVar.ToString().PadLeft( 4, '0' ) + ".imc";
        }

        public string GetVFXPath(int idx )
        {
            return vfxPath + idx.ToString().PadLeft( 4, '0' ) + ".avfx";   
        }
    }
}
