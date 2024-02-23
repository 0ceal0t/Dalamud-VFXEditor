using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Items {
    public class SelectedMdl {
        public bool IsWeapon;
        public string WeaponPath;
        public Dictionary<string, string> ArmorPaths;
    }

    public class ItemtabMdl : ItemTab<SelectedMdl> {
        public ItemtabMdl( SelectDialog dialog, string name ) : base( dialog, name, "Item-Mdl", ItemTabFilter.Weapon | ItemTabFilter.SubWeapon | ItemTabFilter.Armor | ItemTabFilter.Acc ) { }

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
                    ArmorPaths = SelectDataUtils.CharacterRaces.ToDictionary( x => x.Name, x => armor.GetMdlPath( x.Id ) )
                };
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );

            if( Loaded.IsWeapon ) {
                DrawPath( "Model", Loaded.WeaponPath, Selected.Name );
            }
            else {
                DrawPaths( Loaded.ArmorPaths, Selected.Name );
            }
        }
    }
}
