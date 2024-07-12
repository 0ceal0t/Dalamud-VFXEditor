using Lumina.Excel.GeneratedSheets2;
using System.Collections.Generic;
using VfxEditor.Select.Tabs.Items;

namespace VfxEditor.Select.Tabs.Skeleton {
    public class SkeletonTabWeapon : SelectTab<ItemRowWeapon, Dictionary<string, string>> {
        private readonly string Prefix;
        private readonly string Extension;

        public SkeletonTabWeapon( SelectDialog dialog, string name, string prefix, string extension ) : base( dialog, name, "Skeleton-Weapon" ) {
            Prefix = prefix;
            Extension = extension;
        }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var row in Dalamud.DataManager.GetExcelSheet<Item>() ) {
                if( row.EquipSlotCategory.Value?.MainHand == 1 || row.EquipSlotCategory.Value?.OffHand == 1 ) {
                    var weapon = new ItemRowWeapon( row );
                    if( weapon.HasModel ) Items.Add( weapon );
                    if( weapon.HasSubModel ) Items.Add( weapon.SubItem );
                }
            }
        }

        public override void LoadSelection( ItemRowWeapon item, out Dictionary<string, string> loaded ) {
            // chara/weapon/w2251/skeleton/parts/p0066/skl_w2251p0066.sklb
            // chara/weapon/w2251/skeleton/base/b0001/skl_w2251b0001.sklb

            // chara/weapon/w2251/skeleton/base/b0001/phy_w2251b0001.phyb
            // chara/weapon/w2251/skeleton/parts/p0066/phy_w2251p0066.phyb

            var partString = $"p{item.Ids.WeaponBody:D4}";
            loaded = SelectDataUtils.FileExistsFilter( new() {
                { "Base", $"chara/weapon/{item.ModelString}/skeleton/base/b0001/{Prefix}_{item.ModelString}b0001.{Extension}" },
                { "Part", $"chara/weapon/{item.ModelString}/skeleton/parts/{partString}/{Prefix}_{item.ModelString}{partString}.{Extension}" }
            } );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Loaded, Selected.Name, SelectResultType.GameItem );
        }

        protected override bool CheckMatch( ItemRowWeapon item, string searchInput ) => item.CheckMatch( searchInput );
    }
}