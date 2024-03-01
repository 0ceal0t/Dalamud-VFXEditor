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
                ArmorPaths = new(),
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
                    ArmorPaths = SelectDataUtils.CharacterRaces.Select( x => (x.Name, armor.GetMdlPath( x.Id )) ).Where( x => Dalamud.DataManager.FileExists( x.Item2 ) ).ToDictionary( x => x.Name, x => x.Item2 )
                };
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            if( Loaded.IsWeapon ) {
                DrawPaths( Loaded.WeaponPath, Selected.Name );
            }
            else {
                DrawPaths( Loaded.ArmorPaths, Selected.Name );
            }
        }
    }
}
