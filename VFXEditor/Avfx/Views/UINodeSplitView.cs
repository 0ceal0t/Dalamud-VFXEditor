using ImGuiNET;
using System.IO;
using VFXEditor.AVFXLib;

namespace VFXEditor.AVFX.VFX {
    public abstract class UINodeSplitView<T> : UIGenericSplitView, IUINodeView<T> where T : UINode {
        public string Id;
        public AVFXFile Main;
        public AVFXMain AVFX;
        public UINodeGroup<T> Group;
        public T Selected = null;

        public UINodeSplitView( AVFXFile main, AVFXMain avfx, string _id, bool allowNew = true, bool allowDelete = true ) : base( allowNew, allowDelete ) {
            Main = main;
            AVFX = avfx;
            Id = _id;
        }

        public abstract T OnNew();
        public abstract void OnDelete( T item );
        public virtual void OnSelect( T item ) { }
        public abstract T OnImport( BinaryReader reader, int size, bool has_dependencies = false );

        public override void DrawControls( string parentId ) {
            IUINodeView<T>.DrawControls( this, Main, Selected, Group, AllowNew, AllowDelete, parentId );
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
