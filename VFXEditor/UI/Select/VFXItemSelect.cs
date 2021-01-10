using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI
{
    public class VFXItemSelect : VFXSelectTab<XivItem, XivItemSelected> {
        public VFXItemSelect( string parentId, string tabId, List<XivItem> data, Plugin plugin, VFXSelectDialog dialog ) : base(parentId, tabId, data, plugin, dialog) {
        }

        public override bool CheckMatch( XivItem item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawSelected( XivItemSelected loadedItem ) {
            ImGui.Text( loadedItem.Item.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Variant: " + loadedItem.Item.Variant );
            ImGui.Text( "IMC Count: " + loadedItem.Count );
            ImGui.Text( "VFX Id: " + loadedItem.VfxId );

            ImGui.Text( "IMC Path: " );
            ImGui.SameLine();
            _dialog.DisplayPath( loadedItem.ImcPath );

            ImGui.Text( "VFX Path: " );
            ImGui.SameLine();
            _dialog.DisplayPath( loadedItem.GetVFXPath() );
            if( loadedItem.VfxExists ) {
                if( ImGui.Button( "SELECT" + Id ) ) {
                    _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameItem, "[ITEM] " + loadedItem.Item.Name, loadedItem.GetVFXPath() ) );
                }
                ImGui.SameLine();
                _dialog.Copy( loadedItem.GetVFXPath(), id: ( Id + "Copy" ) );
            }
        }

        public override bool SelectItem( XivItem item, out XivItemSelected loadedItem ) {
            return _plugin.Manager.SelectItem( item, out loadedItem );
        }

        public override string UniqueRowTitle( XivItem item ) {
            return item.Name + Id;
        }
    }
}