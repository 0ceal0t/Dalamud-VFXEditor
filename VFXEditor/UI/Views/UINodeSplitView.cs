using AVFXLib.Models;
using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public abstract class UINodeSplitView<T> : UIGenericSplitView, UINodeView<T> where T : UINode {
        public string Id;
        public UIMain Main;
        public AVFXBase AVFX;
        public UINodeGroup<T> Group;
        public T Selected = null;

        public UINodeSplitView( UIMain main, AVFXBase avfx, string _id, bool allowNew = true, bool allowDelete = true) : base(allowNew, allowDelete) {
            Main = main;
            AVFX = avfx;
            Id = _id;
        }

        public abstract T OnNew();
        public abstract void OnDelete( T item );
        public virtual void OnSelect( T item ) { }
        public abstract T OnImport( AVFXLib.AVFX.AVFXNode node, bool has_dependencies = false );

        public override void DrawControls( string parentId ) {
            UINodeView<T>.DrawControls( this, Main, Selected, Group, AllowNew, AllowDelete, parentId );
        }

        public override void DrawLeftCol( string parentId ) {
            for( var idx = 0; idx < Group.Items.Count; idx++ ) {
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

        public void ControlDelete() {
            Selected = null;
        }

        public void ControlCreate() {
            Group.Add( OnNew() );
        }
    }
}
