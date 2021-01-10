using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI {
    public abstract class VFXSelectTab<T, S> {
        public Plugin _plugin;
        public VFXSelectDialog _dialog;
        public string ParentId;
        public string TabId;
        public List<T> Data;
        public string Id;

        public string SearchInput = "";
        public T SelectedZone = default(T);
        public S LoadedZone = default(S);

        public VFXSelectTab( string parentId, string tabId, List<T> data, Plugin plugin, VFXSelectDialog dialog ) {
            _plugin = plugin;
            _dialog = dialog;
            ParentId = parentId;
            TabId = tabId;
            Data = data;
            Id = "##Select/" + tabId + "/" + parentId;
            // =====================
        }

        public abstract bool CheckMatch( T item, string searchInput);
        public abstract string UniqueRowTitle( T item );
        public abstract bool SelectItem( T item, out S loadedItem );
        public abstract void DrawSelected( S loadedItem );

        public virtual void DrawExtra() { }

        public List<T> SearchedZones;
        public void Draw() {
            if( SearchedZones == null ) { SearchedZones = new List<T>(); SearchedZones.AddRange( Data ); }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            bool ResetScroll = false;
            DrawExtra();
            if( ImGui.InputText( "Search" + Id, ref SearchInput, 255 ) ) {
                SearchedZones = Data.Where( x => CheckMatch(x, SearchInput )).ToList();
                ResetScroll = true;
            }
            ImGui.Columns( 2, Id + "Columns", true );
            ImGui.BeginChild( Id + "Tree" );
            VFXSelectDialog.DisplayVisible( SearchedZones.Count, out int preItems, out int showItems, out int postItems, out float itemHeight );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + preItems * itemHeight );
            if( ResetScroll ) { ImGui.SetScrollHereY(); };
            int idx = 0;
            foreach( var item in SearchedZones ) {
                if( idx < preItems || idx > ( preItems + showItems ) ) { idx++; continue; }
                if( ImGui.Selectable( UniqueRowTitle(item), EqualityComparer<T>.Default.Equals(SelectedZone, item) ) ) {
                    if( !EqualityComparer<T>.Default.Equals( SelectedZone, item ) ) {
                        Task.Run( async () => {
                            bool result = SelectItem( item, out LoadedZone );
                        });
                        SelectedZone = item;
                    }
                }
                idx++;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + postItems * itemHeight );
            ImGui.EndChild();
            ImGui.NextColumn();
            // ========================
            if( SelectedZone == null ) {
                ImGui.Text( "Select an item..." );
            }
            else {
                if( LoadedZone != null ) {
                    ImGui.BeginChild( Id + "Selected" );

                    DrawSelected( LoadedZone );

                    ImGui.EndChild();
                }
                else {
                    ImGui.Text( "No data found" );
                }
            }
            ImGui.Columns( 1 );
        }
    }
}