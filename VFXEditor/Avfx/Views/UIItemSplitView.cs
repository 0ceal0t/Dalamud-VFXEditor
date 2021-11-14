using AVFXLib.Models;
using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Helper;

namespace VFXEditor.Avfx.Vfx {
    public class UIItemSplitView<T> : UIGenericSplitView where T : UIItem {
        public List<T> Items;
        private T Selected = null;

        public UIItemSplitView( List<T> items, bool allowNew = false, bool allowDelete = false ) : base( allowNew, allowDelete ) {
            Items = items;
            SetupIdx();
        }
        public void SetupIdx() {
            for( var i = 0; i < Items.Count; i++ ) {
                Items[i].Idx = i;
            }
        }

        public virtual T OnNew() { return null; }
        public virtual void OnDelete( T item ) { }
        public virtual void OnSelect( T item ) { }

        public override void DrawControls( string parentId ) {
            ImGui.PushFont( UiBuilder.IconFont );
            if( AllowNew ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}" + parentId ) ) {
                    var item = OnNew();
                    if( item != null ) {
                        item.Idx = Items.Count;
                        Items.Add( item );
                    }
                }
            }
            if( Selected != null && AllowDelete ) {
                ImGui.SameLine();
                if( UiHelper.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + parentId ) ) {
                    Items.Remove( Selected );
                    OnDelete( Selected );
                    SetupIdx();
                    Selected = null;
                }
            }
            ImGui.PopFont();
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
            if( Selected != null && Selected.Assigned ) {
                Selected.DrawBody( parentId );
            }
        }
    }
}
