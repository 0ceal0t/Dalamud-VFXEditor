using System;
using System.Collections.Generic;
using System.Text;

namespace VFXEditor.Select.Rows {
    public struct XivItemIds {
        public int PrimaryId;
        public int PrimaryVar;
        public int SecondaryId;
        public int SecondaryVar;

        public XivItemIds( ulong modelDataRaw ) {
            /*
             * Gear: [Id, Var, -, -] / [-,-,-,-]
             * Weapon: [Id, Var, Id, -] / [Id, Var, Id, -]
             */
            var b = BitConverter.GetBytes( modelDataRaw );
            PrimaryId = BitConverter.ToInt16( b, 0 ); // primary key
            PrimaryVar = BitConverter.ToInt16( b, 2 ); // primary variant (weapon if != 0)
            SecondaryId = BitConverter.ToInt16( b, 4 ); // secondary key
            SecondaryVar = BitConverter.ToInt16( b, 6 ); // secondary variant
        }

        public static ulong ToItemsId( int primaryId, int primaryVar, int secondaryId, int secondaryVar ) {
            List<byte> bytes = new();
            bytes.AddRange( BitConverter.GetBytes( (short)primaryId ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )primaryVar ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )secondaryId ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )secondaryVar ) );
            return (ulong) BitConverter.ToInt64(bytes.ToArray());
        }
    }

    public class XivItem {
        public bool HasSub;
        public XivItem SubItem = null;

        public string Name;
        public XivItemIds Ids;
        public XivItemIds SecondaryIds;
        public bool HasModel;
        public int Variant;

        public string RootPath;
        public string VfxRootPath;
        public string ImcPath;

        public int RowId;
        public ushort Icon;

        public XivItem( Lumina.Excel.GeneratedSheets.Item item ) {
            Name = item.Name.ToString();
            RowId = ( int )item.RowId;
            Icon = item.Icon;

            Ids = new XivItemIds( item.ModelMain );
            SecondaryIds = new XivItemIds( item.ModelSub );
            HasModel = ( Ids.PrimaryId != 0 );
            HasSub = ( SecondaryIds.PrimaryId != 0 );

            RootPath = "chara/weapon/w" + Ids.PrimaryId.ToString().PadLeft( 4, '0' ) + "/obj/body/b" + Ids.PrimaryVar.ToString().PadLeft( 4, '0' ) + "/";
            VfxRootPath = RootPath + "vfx/eff/vw";
            ImcPath = RootPath + "b" + Ids.PrimaryVar.ToString().PadLeft( 4, '0' ) + ".imc";
            Variant = Ids.SecondaryId;

            if( HasSub ) {
                var category = item.ItemUICategory.Value.RowId;
                var doubleHand = ( category == 1 || category == 84 || category == 107 ); // MNK, NIN, DNC weapons

                var sItem = new Lumina.Excel.GeneratedSheets.Item {
                    Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( Name + " / Offhand" ) ),
                    Icon = item.Icon,
                    EquipRestriction = item.EquipRestriction,
                    EquipSlotCategory = item.EquipSlotCategory,
                    ItemSearchCategory = item.ItemSearchCategory,
                    ItemSortCategory = item.ItemSortCategory,
                    ClassJobCategory = item.ClassJobCategory,
                    ItemUICategory = item.ItemUICategory,
                    ModelMain = doubleHand ? XivItemIds.ToItemsId(Ids.PrimaryId + 50, Ids.PrimaryVar, Ids.SecondaryId, Ids.SecondaryVar) : item.ModelSub, // not sure why this requires it. sometimes the +50 model isn't in the submodel
                    ModelSub = 0
                };
                SubItem = new XivItem( sItem );

                if (doubleHand) SubItem.ImcPath = ImcPath;
            }
        }

        public string GetVFXPath( int idx ) {
            return VfxRootPath + idx.ToString().PadLeft( 4, '0' ) + ".avfx";
        }
    }
}
