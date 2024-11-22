using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Tabs.Items;

namespace VfxEditor.Select.Tabs.Skeleton {
    public class SkeletonTabArmor : SelectTab<ItemRowArmor, Dictionary<string, string>> {
        private readonly string Prefix;
        private readonly string Extension;

        public SkeletonTabArmor( SelectDialog dialog, string name, string prefix, string extension ) : base( dialog, name, "Skeleton-Armor" ) {
            Prefix = prefix;
            Extension = extension;
        }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var row in Dalamud.DataManager.GetExcelSheet<Item>() ) {
                if( row.EquipSlotCategory.ValueNullable?.Head == 1 || row.EquipSlotCategory.ValueNullable?.Body == 1 ) {
                    var armor = new ItemRowArmor( row );
                    if( armor.HasModel ) Items.Add( armor );
                }
            }
        }

        public override void LoadSelection( ItemRowArmor item, out Dictionary<string, string> loaded ) {
            var suffix = item.Suffix;
            var itemString = $"{suffix[0]}{item.Ids.Id:D4}";

            var paths = SelectDataUtils.CharacterRaces.ToDictionary( x => x.Name, x => $"chara/human/{x.Id}/skeleton/{suffix}/{itemString}/{Prefix}_{x.Id}{itemString}.{Extension}" );
            loaded = SelectDataUtils.FileExistsFilter( paths );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Loaded, Selected.Name, SelectResultType.GameItem );
        }

        protected override bool CheckMatch( ItemRowArmor item, string searchInput ) => item.CheckMatch( searchInput );
    }
}