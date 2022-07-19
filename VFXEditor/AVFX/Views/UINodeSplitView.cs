using ImGuiNET;
using System.IO;
using VFXEditor.AVFXLib;

namespace VFXEditor.AVFX.VFX {
    public abstract class UINodeSplitView<T> : UIGenericSplitView, IUINodeView<T> where T : UINode {
        public readonly AVFXFile VfxFile;
        public readonly AVFXMain Avfx;
        public readonly UINodeGroup<T> Group;

        public readonly string Id;

        public T Selected = null;

        public UINodeSplitView( AVFXFile vfxFile, AVFXMain avfx, UINodeGroup<T> group, string name, bool allowNew, bool allowDelete ) : base( allowNew, allowDelete ) {
            VfxFile = vfxFile;
            Avfx = avfx;
            Group = group;
            Id = $"##{name}";
        }

        public abstract T OnNew();
        public abstract void OnDelete( T item );
        public virtual void OnSelect( T item ) { }
        public abstract T OnImport( BinaryReader reader, int size, bool hasDependencies = false );

        public void AddToGroup( T item ) {
            Group.Add( item );
        }

        public override void DrawControls( string parentId ) {
            IUINodeView<T>.DrawControls( this, VfxFile, Selected, Group, AllowNew, AllowDelete, parentId );
        }

        public override void DrawLeftCol( string parentId ) {
            for( var idx = 0; idx < Group.Items.Count; idx++ ) {
                var item = Group.Items[idx];
                if( ImGui.Selectable( $"{item.GetText()}{Id}{idx}", Selected == item ) ) {
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

        public void DeleteSelected() {
            Selected = null;
        }

        public void CreateDefault() => Group.Add( OnNew() );
    }
}
