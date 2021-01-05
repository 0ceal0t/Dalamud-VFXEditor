using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI
{
    public class VFXItemSelect
    {
        public Plugin _plugin;
        public VFXSelectDialog _dialog;
        public string ParentId;
        public string TabId;
        public List<XivItem> Data;
        public string Id;

        public string SearchInput = "";
        public XivItem SelectedItem = null;
        public XivSelectedItem LoadedItem = null;

        public VFXItemSelect( string parentId, string tabId, List<XivItem> data, Plugin plugin, VFXSelectDialog dialog )
        {
            _plugin = plugin;
            _dialog = dialog;
            ParentId = parentId;
            TabId = tabId;
            Data = data;
            Id = "##Select/" + tabId + "/" + parentId;
            // =====================
        }

        public void Draw()
        {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.InputText( "Search" + Id, ref SearchInput, 255 );
            ImGui.Columns( 2, Id + "Columns", true );
            //
            ImGui.BeginChild( Id + "Tree" );
            foreach( var item in _plugin.Manager.Items )
            {
                if( !VFXSelectDialog.Matches( item.Name, SearchInput ) )
                    continue;

                if( ImGui.Selectable( item.Name + Id, SelectedItem == item ) )
                {
                    if( item != SelectedItem )
                    {
                        bool result = _plugin.Manager.SelectItem( item, out LoadedItem );
                        SelectedItem = item;
                    }
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if( SelectedItem == null )
            {
                ImGui.Text( "Select an item..." );
            }
            else
            {
                if( LoadedItem != null )
                {
                    ImGui.Text( LoadedItem.Item.Name );
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                    ImGui.Text( "Variant: " + LoadedItem.Item.Variant );
                    ImGui.Text( "IMC Count: " + LoadedItem.Count );
                    ImGui.Text( "VFX Id: " + LoadedItem.VfxId );

                    ImGui.Text( "IMC Path: " );
                    ImGui.SameLine();
                    _dialog.DisplayPath( LoadedItem.ImcPath );

                    ImGui.Text( "VFX Path: " );
                    ImGui.SameLine();
                    _dialog.DisplayPath( LoadedItem.GetVFXPath() );
                    if( LoadedItem.VfxExists )
                    {
                        if( ImGui.Button( "SELECT" + Id ) )
                        {
                            _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameItem, "[ITEM] " + LoadedItem.Item.Name, LoadedItem.GetVFXPath() ) );
                        }
                        ImGui.SameLine();
                        _dialog.Copy( LoadedItem.GetVFXPath(), id: ( Id + "Copy" ) );
                    }
                }
                else
                {
                    ImGui.Text( "No data found" );
                }
            }
            ImGui.Columns( 1 );
        }
    }
}