using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Items {
    public class SelectedMdl {
        public bool IsWeapon;
        public string WeaponPath;
        public Dictionary<string, string> ArmorPaths;
    }

    public class ItemTabMdl : ItemTab<SelectedMdl> {
        public ItemTabMdl( SelectDialog dialog, string name ) : base( dialog, name, "Item-Mdl", ItemTabFilter.Weapon | ItemTabFilter.SubWeapon | ItemTabFilter.Armor | ItemTabFilter.Acc ) { }

        // ===== LOADING =====

        public override void LoadSelection( ItemRow item, out SelectedMdl loaded ) {
            loaded = new() {
                IsWeapon = false,
                WeaponPath = "",
                ArmorPaths = [],
            };

            if( item is ItemRowWeapon weapon ) {
                loaded = new() {
                    IsWeapon = true,
                    WeaponPath = weapon.MdlPath
                };
            }
            else if( item is ItemRowArmor armor ) {
                loaded = new() {
                    IsWeapon = false,
                    ArmorPaths = SelectDataUtils.CharacterRaces
                        .Select( x => (x.Name, armor.GetMdlPath( x.Id )) )
                        .Where( x => Dalamud.DataManager.FileExists( x.Item2 ) )
                        .ToDictionary( x => x.Name, x => x.Item2 )
                };

                // Weird case
                if( armor.Type == ItemType.RFinger ) {
                    var newArmorPaths = new Dictionary<string, string>();
                    foreach( var (name, path) in loaded.ArmorPaths ) {
                        newArmorPaths[$"{name} (Right)"] = path;
                        var leftPath = path.Replace( "_rir", "_ril" );
                        if( Dalamud.DataManager.FileExists( leftPath ) ) newArmorPaths[$"{name} (Left)"] = leftPath;
                    }
                    loaded.ArmorPaths = newArmorPaths;
                }
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            if( Loaded.IsWeapon ) {
                Dialog.DrawPaths( Loaded.WeaponPath, Selected.Name, SelectResultType.GameItem );
            }
            else {
                Dialog.DrawPaths( Loaded.ArmorPaths, Selected.Name, SelectResultType.GameItem );
            }
        }
    }
}
