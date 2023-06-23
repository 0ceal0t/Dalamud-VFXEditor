using System.Collections.Generic;
using VfxEditor.Select.Shared.Item;

namespace VfxEditor.Select.Phyb.Armor {
    public class ArmorRowSelected {
        public Dictionary<string, string> Paths;
    }

    public class ArmorTab : SelectTab<ArmorRow, Dictionary<string, string>> {
        public ArmorTab( SelectDialog dialog, string name ) : base( dialog, name, "Phyb-Armor" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var row in Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>() ) {
                if( row.EquipSlotCategory.Value?.Head == 1 || row.EquipSlotCategory.Value?.Body == 1 ) {
                    var armor = new ArmorRow( row );
                    if( armor.HasModel ) Items.Add( armor );
                }
            }
        }

        public override void LoadSelection( ArmorRow item, out Dictionary<string, string> loaded ) {
            // chara/human/c0801/skeleton/top/t0748/phy_c0801t0748.phyb
            // chara/human/c0801/skeleton/met/m6173/phy_c0801m6173.phyb

            var armorString = item.Type switch {
                ItemType.Body => "top",
                ItemType.Head => "met",
                _ => "unk"
            };

            var armorPrefix = armorString[0];
            var itemString = $"{armorPrefix}{item.Ids.Id:D4}";

            var paths = new Dictionary<string, string>();
            foreach( var race in SelectUtils.RaceAnimationIds ) {
                paths[race.Key] = $"chara/human/{race.Value.SkeletonId}/skeleton/{armorString}/{itemString}/phy_{race.Value.SkeletonId}{itemString}.phyb";
            }
            loaded = SelectUtils.FileExistsFilter( paths );
        }

        protected override string GetName( ArmorRow item ) => item.Name;

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected() {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPaths( Loaded, SelectResultType.GameItem, Selected.Name );
        }
    }
}
