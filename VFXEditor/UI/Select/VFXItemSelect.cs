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

        public List<XivItem> SearchedItems;
        public void Draw()
        {
            if(SearchedItems == null ) { SearchedItems = new List<XivItem>(); SearchedItems.AddRange( Data ); }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            bool ResetScroll = false;
            if(ImGui.InputText( "Search" + Id, ref SearchInput, 255 ) )
            {
                SearchedItems = Data.Where( x => VFXSelectDialog.Matches( x.Name, SearchInput ) ).ToList();
                ResetScroll = true;
            }
            ImGui.Columns( 2, Id + "Columns", true );
            ImGui.BeginChild( Id + "Tree" );
            VFXSelectDialog.DisplayVisible(SearchedItems.Count, out int preItems, out int showItems, out int postItems, out float itemHeight );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + preItems * itemHeight );
            if( ResetScroll ) { ImGui.SetScrollHereY(); };
            int idx = 0;
            foreach( var item in SearchedItems )
            {
                if( idx < preItems || idx > ( preItems + showItems ) ) { idx++; continue; }
                if( ImGui.Selectable( item.Name + Id, SelectedItem == item ) )
                {
                    if( item != SelectedItem )
                    {
                        bool result = _plugin.Manager.SelectItem( item, out LoadedItem );
                        SelectedItem = item;
                    }
                }
                idx++;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + postItems * itemHeight );
            ImGui.EndChild();
            ImGui.NextColumn();
            // ========================
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