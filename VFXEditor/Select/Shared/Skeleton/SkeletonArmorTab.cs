using System.Collections.Generic;
using VfxEditor.Select.Shared.Item;

namespace VfxEditor.Select.Shared.Skeleton {
    public class ArmorRowSelected {
        public Dictionary<string, string> Paths;
    }

    public class SkeletonArmorTab : SelectTab<ArmorRow, Dictionary<string, string>> {
        private readonly string Prefix;
        private readonly string Extension;

        public SkeletonArmorTab( SelectDialog dialog, string name, string prefix, string extension ) : base( dialog, name, "Skeleton-Armor", SelectResultType.GameItem ) {
            Prefix = prefix;
            Extension = extension;
        }

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
            var armorString = item.Type switch {
                ItemType.Body => "top",
                ItemType.Head => "met",
                _ => "unk"
            };
            var itemString = $"{armorString[0]}{item.Ids.Id:D4}";

            var paths = new Dictionary<string, string>();
            foreach( var race in SelectDataUtils.RaceAnimationIds ) {
                paths[race.Key] = $"chara/human/{race.Value.SkeletonId}/skeleton/{armorString}/{itemString}/{Prefix}_{race.Value.SkeletonId}{itemString}.{Extension}";
            }
            loaded = SelectDataUtils.FileExistsFilter( paths );
        }

        protected override string GetName( ArmorRow item ) => item.Name;

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected() {
            SelectUiUtils.DrawIcon( Icon );

            DrawPaths( Loaded, Selected.Name );
        }
    }
}
