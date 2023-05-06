using Dalamud.Logging;
using ImGuiNET;
using Lumina.Data.Files;
using System;

namespace VfxEditor.Select.Vfx.Item {
    public class ItemTab : SelectTab<ItemRow, ItemRowSelected> {
        public ItemTab( SelectDialog dialog, string name ) : base( dialog, name, "Vfx-Item" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var row in Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>() ) {
                if( row.EquipSlotCategory.Value?.MainHand == 1 || row.EquipSlotCategory.Value?.OffHand == 1 ) {
                    var weapon = new WeaponRow( row );
                    if( weapon.HasModel ) Items.Add( weapon );
                    if( weapon.HasSubModel ) Items.Add( weapon.SubItem );
                }
                else if(
                    row.EquipSlotCategory.Value?.Head == 1 ||
                    row.EquipSlotCategory.Value?.Body == 1 ||
                    row.EquipSlotCategory.Value?.Gloves == 1 ||
                    row.EquipSlotCategory.Value?.Legs == 1 ||
                    row.EquipSlotCategory.Value?.Feet == 1
                ) {
                    var armor = new ArmorRow( row );
                    if( armor.HasModel ) Items.Add( armor );
                }
            }
        }

        public override void LoadSelection( ItemRow item, out ItemRowSelected loaded ) {
            loaded = null;
            var imcPath = item.ImcPath;
            if( Plugin.DataManager.FileExists( imcPath ) ) {
                try {
                    var file = Plugin.DataManager.GetFile<ImcFile>( imcPath );
                    loaded = new ItemRowSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error loading IMC file " + imcPath );
                }
            }
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            ImGui.Text( "Variant: " + Selected.Variant );
            ImGui.Text( "IMC: " );
            ImGui.SameLine();
            SelectTabUtils.DisplayPath( Loaded.ImcPath );

            Dialog.DrawPaths( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameItem, Selected.Name, true );

            if( Loaded.VfxPaths.Count == 0 ) SelectTabUtils.DisplayNoVfx();
        }

        protected override string GetName( ItemRow item ) => item.Name;
    }
}
