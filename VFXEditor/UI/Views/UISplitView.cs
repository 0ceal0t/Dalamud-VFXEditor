using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UISplitView<T> : UIBase where T : UIItem
    {
        public List<T> Items;
        public bool NewButton;
        public UIItem SelectedItem = null;
        public int LeftSize;

        public UISplitView(List<T> items, bool newButton = false, int leftSize = 200)
        {
            Items = items;
            NewButton = newButton;
            LeftSize = leftSize;
        }

        bool DrawOnce = false;
        public override void Draw( string parentId )
        {
            ImGui.Columns( 2, parentId + "/Cols", true );
            // ===== C1 =========
            if( NewButton ) {
                DrawNewButton( parentId );
            }
            ImGui.BeginChild( parentId + "/Tree" );
            // assigned, good to go
            for(int idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                if( item.Assigned ) {
                    item.DrawSelect( idx, parentId, ref SelectedItem );
                }
            }
            // not assigned, can be added
            for( int idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                if( !item.Assigned )
                    item.DrawSelect( idx, parentId, ref SelectedItem );
            }
            ImGui.EndChild();
            if( !DrawOnce ) {
                ImGui.SetColumnWidth( 0, LeftSize );
                DrawOnce = true;
            }
            // ===== C2 ============
            ImGui.NextColumn();
            ImGui.BeginChild( parentId + "/Split" );
            if( SelectedItem != null && SelectedItem.Assigned == true )
            {
                SelectedItem.DrawBody( parentId );
            }
            ImGui.EndChild();
            ImGui.Columns( 1 );
        }

        public void OnNew(T item ) {
            Items.Add( item );
            // TODO: fix IDX
        }
        public void OnDelete(T item ) {
            Items.Remove( item );
            SelectedItem = null;
            // TODO: fix IDX
        }

        public virtual void DrawNewButton(string parentId ) { }
    }
}
