using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class ItemSheetLoader : SheetLoader<XivItem, XivItemSelected> {
        public override void OnLoad() {
            foreach( var row in Plugin.DataManager.GetExcelSheet<Item>() ) {
                if( row.EquipSlotCategory.Value?.MainHand == 1 || row.EquipSlotCategory.Value?.OffHand == 1 ) {
                    var item = new XivWeapon( row );
                    if( item.HasModel ) Items.Add( item );
                    if( item.HasSubModel ) Items.Add( item.SubItem );
                }
                else if(
                    row.EquipSlotCategory.Value?.Head == 1 ||
                    row.EquipSlotCategory.Value?.Body == 1 ||
                    row.EquipSlotCategory.Value?.Gloves == 1 ||
                    row.EquipSlotCategory.Value?.Legs == 1 ||
                    row.EquipSlotCategory.Value?.Feet == 1
                ) {
                    var i = new XivArmor( row );
                    if( i.HasModel ) Items.Add( i );
                }
            }
        }

        public override bool SelectItem( XivItem item, out XivItemSelected selectedItem ) {
            selectedItem = null;
            var imcPath = item.ImcPath;
            var result = Plugin.DataManager.FileExists( imcPath );
            if( result ) {
                try {
                    var file = Plugin.DataManager.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
                    selectedItem = new XivItemSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error loading IMC file " + imcPath );
                    return false;
                }
            }
            return result;
        }
    }
}
