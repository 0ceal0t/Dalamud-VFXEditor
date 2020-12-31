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
        public bool IsWeapon;

        public XivItem(Lumina.Excel.GeneratedSheets.Item item)
        {
            Name = CleanSEString(item.Name);

            Ids = new XivItemIds( item.ModelMain );
            SecondaryIds = new XivItemIds( item.ModelSub );
            HasModel = ( Ids.PrimaryId != 0 );
            HasSub = ( SecondaryIds.PrimaryId != 0 );
            IsWeapon = HasModel && ( Ids.SecondaryId != 0 );

            //if( Name[0] == 'U' )
            //{
                PluginLog.Log( Name + " ModelId:" + Ids.PrimaryId + " RowId:" + item.RowId);
            //}

            /*if( HasSub )
            {
                item.Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( "Offhand" ) );
                item.ModelMain = item.ModelSub;
                item.ModelSub = 0;
                SubItem = new XivItem( item );
            }*/

            //PluginLog.Log( item.ItemUICategory.Value.Name );

            //var iPrefix = XivItemTypes.GetSystemPrefix(Info.PrimaryType);
            //var iId = Info.PrimaryId.ToString().PadLeft( 4, '0' );
            //imcPath = Info.GetRootFolder() + String.Format( ImcFileFormat, new string[] { iPrefix, iId } );

            // chara/weapon/w2101/obj/body/b0006/b0006.imc
            // https://github.com/TexTools/xivModdingFramework/blob/872329d84c7b920fe2ac5e0b824d6ec5b68f4f57/xivModdingFramework/Resources/XivStrings.resx

            // chara/weapon/w2101/obj/body/b0006/ = root folder
            // chara/weapon/w2101/obj/body/b0006/ vfx/eff/vw0002.avfx
            //string ImcFileFormat = "{0}{1}.imc";
        }

        public static string CleanSEString(Lumina.Text.SeString s )
        {
            var st = s.RawString;
            return st.Replace( "\0", string.Empty );
        }
    }

    /*
     * try
                {
                    // This checks whether there is any model data present in the current item
                    if (item.Value[modelDataCheckOffset] <= 0 && item.Value[modelDataCheckOffset + 1] <= 0) return;

                    var primaryMi = new XivGearModelInfo();
                    var secondaryMi = new XivGearModelInfo();
                    var hasSecondary = false;

                    var xivGear = new XivGear
                    {
                        ExdID = item.Key,
                        PrimaryCategory = XivStrings.Gear,
                        ModelInfo = primaryMi,
                    };
Used to determine if the given model is a weapon
                     * This is important because the data is formatted differently
                     * The model data is a 16 byte section separated into two 8 byte parts (primary model, secondary model)
                     * Format is 8 bytes in length with 2 bytes per data point [short, short, short, short]
                     * Gear: primary model [blank, blank, variant, ID] nothing in secondary model
                     * Weapon: primary model [blank, variant, body, ID] secondary model [blank, variant, body, ID]
    var isWeapon = false;

                    // Big Endian Byte Order 
                    using (var br = new BinaryReaderBE(new MemoryStream( item.Value)))
                    {
                        br.BaseStream.Seek(nameDataOffset, SeekOrigin.Begin);
                        var nameOffset = br.ReadInt16();

    // Model Data
    br.BaseStream.Seek(modelDataOffset, SeekOrigin.Begin);

                        // Primary Model Key
                        primaryMi.ModelKey = Quad.Read(br.ReadBytes(8), 0);
                        br.BaseStream.Seek(-8, SeekOrigin.Current);

                        // Primary Blank
                        var unused = br.ReadInt16();

    // Primary Variant for weapon, blank otherwise
    var weaponVariant = br.ReadInt16();

                        if (weaponVariant != 0)
                        {
                            primaryMi.ImcSubsetID = weaponVariant;
                            primaryMi.IsWeapon = true;
                            isWeapon = true;
                        }

// Primary Body if weapon, Variant otherwise
if( isWeapon )
{
    primaryMi.SecondaryID = br.ReadInt16();
}
else
{
    primaryMi.ImcSubsetID = br.ReadInt16();
}

// Primary Model ID
primaryMi.PrimaryID = br.ReadInt16();

// Secondary Model Key
isWeapon = false;
secondaryMi.ModelKey = Quad.Read( br.ReadBytes( 8 ), 0 );
br.BaseStream.Seek( -8, SeekOrigin.Current );

// Secondary Blank
var unused2 = br.ReadInt16();

// Secondary Variant for weapon, blank otherwise
weaponVariant = br.ReadInt16();

if( weaponVariant != 0 )
{
    secondaryMi.ImcSubsetID = weaponVariant;
    secondaryMi.IsWeapon = true;
    isWeapon = true;
}

// Secondary Body if weapon, Variant otherwise
if( isWeapon )
{
    secondaryMi.SecondaryID = br.ReadInt16();
}
else
{
    secondaryMi.ImcSubsetID = br.ReadInt16();
}

// Secondary Model ID
secondaryMi.PrimaryID = br.ReadInt16();

// Icon
br.BaseStream.Seek( iconDataOffset, SeekOrigin.Begin );
xivGear.IconNumber = br.ReadUInt16();

// Gear Slot/Category
br.BaseStream.Seek( slotDataOffset, SeekOrigin.Begin );
int slotNum = br.ReadByte();

// Waist items do not have texture or model data
if( slotNum == 6 ) return;

xivGear.EquipSlotCategory = slotNum;
xivGear.SecondaryCategory = _slotNameDictionary.ContainsKey( slotNum ) ? _slotNameDictionary[slotNum] : "Unknown";

// Gear Name
var gearNameOffset = dataLength + nameOffset;
var gearNameLength = item.Value.Length - gearNameOffset;
br.BaseStream.Seek( gearNameOffset, SeekOrigin.Begin );
var nameString = Encoding.UTF8.GetString( br.ReadBytes( gearNameLength ) ).Replace( "\0", "" );
xivGear.Name = new string( nameString.Where( c => !char.IsControl( c ) ).ToArray() );
xivGear.Name = xivGear.Name.Trim();

// If we have a secondary model

XivGear secondaryItem = null;
if( secondaryMi.PrimaryID != 0 )
{
    // Make a new item for it.
    secondaryItem = ( XivGear )xivGear.Clone();
    secondaryItem.ModelInfo = secondaryMi;
    xivGear.Name += " - " + XivStrings.Main_Hand;
    secondaryItem.Name += " - " + XivStrings.Off_Hand;
    xivGear.PairedItem = secondaryItem;
    secondaryItem.PairedItem = xivGear;
    xivGear.SecondaryCategory = XivStrings.Dual_Wield;
    secondaryItem.SecondaryCategory = XivStrings.Dual_Wield;
}

// Rings
if( slotNum == 12 )
{
    // Make this the Right ring, and create the Left Ring entry.
    secondaryItem = ( XivGear )xivGear.Clone();

    xivGear.Name += " - " + XivStrings.Right;
    secondaryItem.Name += " - " + XivStrings.Left;

    xivGear.PairedItem = secondaryItem;
    secondaryItem.PairedItem = xivGear;
}

lock( _gearLock )
{
    xivGearList.Add( xivGear );
    if( secondaryItem != null )
    {
        xivGearList.Add( secondaryItem );
    }
}
                    }
                } catch( Exception ex )
{
    throw;
}
* 
     * 
     * public static XivItemType GetPrimaryItemType(this IItem item)
        {
            XivItemType itemType;

            if (item.PrimaryCategory == null || item.SecondaryCategory == null) return XivItemType.unknown;

            if (item.SecondaryCategory.Equals(XivStrings.Main_Hand) || item.SecondaryCategory.Equals(XivStrings.Off_Hand) || 
                item.SecondaryCategory.Equals(XivStrings.Main_Off) || item.SecondaryCategory.Equals(XivStrings.Two_Handed) || item.SecondaryCategory.Equals(XivStrings.Dual_Wield) || item.SecondaryCategory.Equals(XivStrings.Food))
            {
                itemType = XivItemType.weapon;

                try
                {
                    // Check to see if we're an equipment item masquerading as a weapon.
                    var mi = (XivGearModelInfo)((IItemModel)item).ModelInfo;
                    if (mi != null)
                    {
                        if (!mi.IsWeapon)
                        {
                            itemType = XivItemType.equipment;
                        }
                    }
                }
                catch
                {
                    //No-op.
                }
            }
            else if (item.PrimaryCategory.Equals(XivStrings.Gear) && (item.SecondaryCategory.Equals(XivStrings.Earring) || item.SecondaryCategory.Equals(XivStrings.Neck) || 
                     item.SecondaryCategory.Equals(XivStrings.Wrists) || item.SecondaryCategory.Equals(XivStrings.Rings) || item.SecondaryCategory.Equals(XivStrings.LeftRing)))
            {
                itemType = XivItemType.accessory;
            }
            else if (item.SecondaryCategory.Equals(XivStrings.Mounts) || item.SecondaryCategory.Equals(XivStrings.Minions) || item.SecondaryCategory.Equals(XivStrings.Pets)
                     || item.SecondaryCategory.Equals(XivStrings.Monster) || item.SecondaryCategory.Equals(XivStrings.Ornaments))
            {
                // This is a little squiggly.  Monster/Demihuman support needs work across the board though
                // So not going to worry about making this better just yet.
                try
                {
                    var modelInfo = (XivMonsterModelInfo)(((IItemModel)item).ModelInfo);
                    itemType = modelInfo.ModelType;
                } catch(Exception ex)
                {
                    itemType = XivItemType.monster;
                }
            }
            else if (item.SecondaryCategory.Equals(XivStrings.DemiHuman))
            {
                itemType = XivItemType.demihuman;
            }
            else if (item.PrimaryCategory.Equals(XivStrings.Character))
            {
                itemType = XivItemType.human;
            }
            else if (item.SecondaryCategory.Equals("UI"))
            {
                itemType = XivItemType.ui;
            }
            else if (item.PrimaryCategory.Equals(XivStrings.Housing))
            {
                if(item.SecondaryCategory.Equals(XivStrings.Paintings))
                {
                    itemType = XivItemType.ui;
                } else
                {
                    itemType = XivItemType.furniture;
                }
            } else if(item.SecondaryCategory.Equals(XivStrings.Equipment_Decals) || item.SecondaryCategory.Equals(XivStrings.Face_Paint))
            {
                itemType = XivItemType.decal;
            }
            else
            {
                itemType = XivItemType.equipment;
            }

            return itemType;
        }

        /// <summary>
        /// Retrieves an item's secondary item type based on it's category information.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static XivItemType GetSecondaryItemType(this IItem item)
        {
            var itemType = XivItemType.none;

            // Weapons, Monsters of all kinds, and the character Body use the body type secondary identifier.
            if (item.SecondaryCategory.Equals(XivStrings.Main_Hand) || item.SecondaryCategory.Equals(XivStrings.Off_Hand) || item.SecondaryCategory.Equals(XivStrings.Dual_Wield)
                || item.SecondaryCategory.Equals(XivStrings.Main_Off) || item.SecondaryCategory.Equals(XivStrings.Two_Handed) || item.SecondaryCategory.Equals(XivStrings.Food)
                || item.PrimaryCategory.Equals(XivStrings.Companions) || item.PrimaryCategory.Equals(XivStrings.Monster) || item.SecondaryCategory.Equals(XivStrings.Body))
            {
                itemType = XivItemType.body;
            } else if(item.SecondaryCategory.Equals( XivStrings.Face ))
            {
                itemType = XivItemType.face;
            }
            else if (item.SecondaryCategory.Equals( XivStrings.Tail ))
            {
                itemType = XivItemType.tail;
            }
            else if (item.SecondaryCategory.Equals(XivStrings.Hair))
            {
                itemType = XivItemType.hair;
            }
            else if (item.PrimaryCategory.Equals(XivStrings.Character) && item.SecondaryCategory.Equals( XivStrings.Ear ))
            {
                itemType = XivItemType.ear;
            }
            else if (item.SecondaryCategory.Equals( XivStrings.DemiHuman))
            {
                itemType = XivItemType.equipment;
            }

            return itemType;
        }
     */
}
