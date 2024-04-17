using Lumina.Excel.GeneratedSheets2;
using System;
using System.Collections.Generic;
using VfxEditor.Select.Base;

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

    public abstract class ItemRow : ISelectItemWithIcon {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;
        public readonly ItemIds Ids;
        public readonly ItemIds SecondaryIds;
        public readonly ItemType Type = ItemType.Other;
        public string ModelString { get; protected set; }

        public bool HasModel => Ids.Id1 != 0;

        public string VariantString => $"v{Variant:D4}";

        public abstract string RootPath { get; }

        public abstract string ImcPath { get; }

        public abstract int Variant { get; }

        public ItemRow( string name, uint rowId, ushort icon, ItemIds ids, ItemIds secondaryIds, EquipSlotCategory category ) {
            Name = name;
            RowId = ( int )rowId;
            Icon = icon;
            Ids = ids;
            SecondaryIds = secondaryIds;

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

        public ItemRow( Item item ) : this( item.Name.ToString(), item.RowId, item.Icon, new( item.ModelMain ), new( item.ModelSub ), item.EquipSlotCategory.Value ) { }

        public string GetVfxPath( int idx ) => $"{RootPath}{idx:D4}.avfx";

        public string GetName() => Name;

        public uint GetIconId() => Icon;
    }
}