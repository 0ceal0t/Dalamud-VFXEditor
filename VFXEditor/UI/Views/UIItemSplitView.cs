using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public class UIItemSplitView<T> : UIGenericSplitView where T : UIItem{
        public List<T> Items;
        public T Selected = null;

        public UIItemSplitView(List<T> items, bool allowNew = false, bool allowDelete = false, int leftSize = 200) : base(allowNew, allowDelete, leftSize ) {
            Items = items;
            SetupIdx();
        }
        public void SetupIdx() {
            for( int i = 0; i < Items.Count; i++ ) {
                Items[i].Idx = i;
            }
        }

        public virtual T OnNew() { return null; }
        public virtual void OnDelete( T item ) { }
        public virtual void OnSelect( T item ) { }

        public override void DrawNewButton( string parentId ) {
            if( ImGui.SmallButton( "+ New" + parentId ) ) {
                var item = OnNew();
                if( item != null ) {
                    item.Idx = Items.Count;
                    Items.Add( item );
                }
            }
        }

        public override void DrawDeleteButton( string parentId ) {
            if( Selected != null && AllowDelete ) {
                if( UIUtils.RemoveButton( "Delete" + parentId, small: true ) ) {
                    Items.Remove( Selected );
                    OnDelete( Selected );
                    SetupIdx();
                    Selected = null;
                }
            }
        }

        public override void DrawLeftCol( string parentId ) {
            foreach( var item in Items.Where( x => x.Assigned ) ) {
                if( ImGui.Selectable( item.GetText() + parentId, Selected == item ) ) {
                    OnSelect( item );
                    Selected = item;
                }
            }
            // not assigned, can be added
            foreach( var item in Items.Where( x => !x.Assigned ) ) {
                item.DrawUnAssigned( parentId );
            }
        }

        public override void DrawRightCol( string parentId ) {
            if( Selected != null && Selected.Assigned) {
                Selected.DrawBody( parentId );
            }
        }
    }
}
