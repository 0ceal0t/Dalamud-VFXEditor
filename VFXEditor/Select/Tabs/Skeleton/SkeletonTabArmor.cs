using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using VfxEditor.Select.Tabs.Items;

namespace VfxEditor.Select.Tabs.Skeleton {
    public class SkeletonTabArmor : SelectTab<ItemRowArmor, Dictionary<string, string>> {
        private readonly string Prefix;
        private readonly string Extension;

        public SkeletonTabArmor( SelectDialog dialog, string name, string prefix, string extension ) : base( dialog, name, "Skeleton-Armor", SelectResultType.GameItem ) {
            Prefix = prefix;
            Extension = extension;
        }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var row in Dalamud.DataManager.GetExcelSheet<Item>() ) {
                if( row.EquipSlotCategory.Value?.Head == 1 || row.EquipSlotCategory.Value?.Body == 1 ) {
                    var armor = new ItemRowArmor( row );
                    if( armor.HasModel ) Items.Add( armor );
                }
            }
        }

        public override void LoadSelection( ItemRowArmor item, out Dictionary<string, string> loaded ) {
            var suffix = item.Suffix;
            var itemString = $"{suffix[0]}{item.Ids.Id:D4}";

            var paths = new Dictionary<string, string>();
            foreach( var race in SelectDataUtils.RaceAnimationIds ) {
                paths[race.Key] = $"chara/human/{race.Value.SkeletonId}/skeleton/{suffix}/{itemString}/{Prefix}_{race.Value.SkeletonId}{itemString}.{Extension}";
            }
            loaded = SelectDataUtils.FileExistsFilter( paths );
        }

        protected override string GetName( ItemRowArmor item ) => item.Name;

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            DrawPaths( Loaded, Selected.Name );
        }
    }
}