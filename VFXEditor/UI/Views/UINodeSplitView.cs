using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public abstract class UINodeSplitView<T> : UIGenericSplitView where T : UINode {
        public string Id;
        public AVFXBase AVFX;
        public UINodeGroup<T> Group;
        public T Selected = null;

        public UINodeSplitView( AVFXBase avfx, string _id, bool allowNew = true, bool allowDelete = true, int leftSize = 200 ) : base(allowNew, allowDelete, leftSize) {
            AVFX = avfx;
            Id = _id;
        }

        public abstract T OnNew();
        public abstract void OnDelete( T item );
        public virtual void OnSelect( T item ) { }

        public override void DrawDeleteButton( string parentId ) {
            if( Selected != null && AllowDelete ) {
                if( UIUtils.RemoveButton( "Delete" + Id, small: true ) ) {
                    Group.Remove( Selected );
                    Selected.DeleteNode();
                    OnDelete( Selected );
                    Selected = null;
                }
            }
        }

        public override void DrawNewButton( string parentId ) {
            if( ImGui.SmallButton( "+ New" + Id ) ) {
                Group.Add( OnNew() );
            }
        }

        public override void DrawLeftCol( string parentId ) {
            for( int idx = 0; idx < Group.Items.Count; idx++ ) {
                var item = Group.Items[idx];
                if( ImGui.Selectable( item.GetText() + Id, Selected == item ) ) {
                    Selected = item;
                    OnSelect( item );
                }
            }
        }

        public override void DrawRightCol( string parentId ) {
            if( Selected != null ) {
                Selected.DrawBody( Id );
            }
        }
    }
}
