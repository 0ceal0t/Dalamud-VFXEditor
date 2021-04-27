using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI
{
    public class VFXItemSelect : VFXSelectTab<XivItem, XivItemSelected> {
        public VFXItemSelect( string parentId, string tabId, SheetManager sheet, VFXSelectDialog dialog ) : 
            base(parentId, tabId, sheet._Items, sheet._pi, dialog) {
        }

        ImGuiScene.TextureWrap Icon;
        public override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        public override bool CheckMatch( XivItem item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawSelected( XivItemSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Item.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Icon != null ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

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

        public override string UniqueRowTitle( XivItem item ) {
            return item.Name + Id;
        }
    }
}