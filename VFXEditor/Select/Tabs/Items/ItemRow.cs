using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Items {
    public struct ItemIds {
        public ulong Raw;

        public int Id1;
        public int Id2;
        public int Id3;
        public int Id4;

        public readonly int Id => Id1;
        public readonly int GearVariant => Id2;
        public readonly int WeaponBody => Id2;
        public readonly int WeaponVariant => Id3;

        public ItemIds( ulong modelDataRaw ) {
            Raw = modelDataRaw;
            /*
             * Gear: [Id, Var, -, -] / [-,-,-,-]
             * Weapon: [Id, Body, Var, -] / [Id, Body, Var, -]
             */
            var bytes = BitConverter.GetBytes( modelDataRaw );
            Id1 = BitConverter.ToInt16( bytes, 0 );
            Id2 = BitConverter.ToInt16( bytes, 2 );
            Id3 = BitConverter.ToInt16( bytes, 4 );
            Id4 = BitConverter.ToInt16( bytes, 6 );
        }


        public static ulong ToLong( int id1, int id2, int id3, int id4 ) {
            var bytes = new List<byte>();
            bytes.AddRange( BitConverter.GetBytes( ( short )id1 ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )id2 ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )id3 ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )id4 ) );
            return ( ulong )BitConverter.ToInt64( bytes.ToArray() );
        }
    }

    public enum ItemType {
        MainHand,
        OffHand,
        Head,
        Body,
        Hands,
        Legs,
        Feet,
        Neck,
        RFinger,
        LFinger,
        Wrists,
        Ears,
        Other
    }

    public abstract class ItemRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;
        public readonly ItemIds Ids;
        public readonly ItemIds SecondaryIds;
        public readonly bool HasModel;
        public readonly ItemType Type = ItemType.Other;

        public string VariantString => "v" + Variant.ToString().PadLeft( 4, '0' );

        public abstract string RootPath { get; }

        public abstract string ImcPath { get; }

        public abstract int Variant { get; }

        public ItemRow( Item item ) {
            Name = item.Name.ToString();
            RowId = ( int )item.RowId;
            Icon = item.Icon;

            Ids = new ItemIds( item.ModelMain );
            SecondaryIds = new ItemIds( item.ModelSub );
            HasModel = Ids.Id1 != 0;

            var category = item.EquipSlotCategory.Value;
            if( category?.MainHand == 1 ) Type = ItemType.MainHand;
            else if( category?.OffHand == 1 ) Type = ItemType.OffHand;
            else if( category?.Head == 1 ) Type = ItemType.Head;
            else if( category?.Body == 1 ) Type = ItemType.Body;
            else if( category?.Gloves == 1 ) Type = ItemType.Hands;
            else if( category?.Legs == 1 ) Type = ItemType.Legs;
            else if( category?.Feet == 1 ) Type = ItemType.Feet;
            else if( category?.Neck == 1 ) Type = ItemType.Neck;
            else if( category?.FingerR == 1 ) Type = ItemType.RFinger;
            else if( category?.FingerL == 1 ) Type = ItemType.LFinger;
            else if( category?.Wrists == 1 ) Type = ItemType.Wrists;
            else if( category?.Ears == 1 ) Type = ItemType.Ears;
        }

        public string GetVfxPath( int idx ) => $"{RootPath}{idx.ToString().PadLeft( 4, '0' )}.avfx";
    }
}