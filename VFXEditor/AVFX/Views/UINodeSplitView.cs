using ImGuiNET;
using System.IO;
using VFXEditor.AVFXLib;
using VFXEditor.Utils;

namespace VFXEditor.AVFX.VFX {
    public abstract class UINodeSplitView<T> : UIGenericSplitView, IUINodeView<T> where T : UINode {
        public readonly AVFXFile VfxFile;
        public readonly AVFXMain Avfx;
        public readonly UINodeGroup<T> Group;

        public readonly string Id;
        private readonly string DefaultText;
        private readonly string DefaultPath;

        public T Selected = null;

        public UINodeSplitView( AVFXFile vfxFile, AVFXMain avfx, UINodeGroup<T> group, string name, bool allowNew, bool allowDelete, string defaultPath ) : base( allowNew, allowDelete ) {
            VfxFile = vfxFile;
            Avfx = avfx;
            Group = group;
            AllowNew = allowNew;
            AllowDelete = allowDelete;

            Id = $"##{name}";
            DefaultText = $"Select {UiUtils.GetArticle( name )} {name}";
            DefaultPath = Path.Combine( Plugin.RootLocation, "Files", defaultPath );
        }

        public abstract void OnDelete( T item );
        public abstract void OnSelect( T item );
        public abstract T OnImport( BinaryReader reader, int size, bool hasDependencies = false );
        public void OnNew() => VfxFile.Import( DefaultPath );

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
                Selected.DrawInline( Id );
            }
            else {
                ImGui.Text( DefaultText );
            }
        }

        public void DeleteSelected() {
            Selected = null;
        }
    }
}
