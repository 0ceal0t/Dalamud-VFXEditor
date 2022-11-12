using ImGuiNET;
using System.IO;
using VfxEditor.AVFXLib;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public abstract class UiNodeSplitView<T> : UiGenericSplitView, IUiNodeView<T> where T : UiNode {
        public readonly AvfxFile VfxFile;
        public readonly AVFXMain Avfx;
        public readonly UiNodeGroup<T> Group;

        public readonly string Id;
        private readonly string DefaultText;
        private readonly string DefaultPath;

        public T Selected = null;

        public UiNodeSplitView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<T> group, string name, bool allowNew, bool allowDelete, string defaultPath ) : base( allowNew, allowDelete ) {
            VfxFile = vfxFile;
            Avfx = avfx;
            Group = group;
            AllowNew = allowNew;
            AllowDelete = allowDelete;

            Id = $"##{name}";
            DefaultText = $"Select {UiUtils.GetArticle( name )} {name}";
            DefaultPath = Path.Combine( Plugin.RootLocation, "Files", defaultPath );
        }

        public abstract void RemoveFromAvfx( T item );
        public abstract void AddToAvfx( T item, int idx );
        public abstract T AddToAvfx( BinaryReader reader, int size, bool hasDependencies );

        public abstract void OnSelect( T item );
        public void ImportDefault() => VfxFile.Import( DefaultPath );

        public void AddToGroup( T item ) => Group.AddAndUpdate( item );

        public override void DrawControls( string parentId ) {
            IUiNodeView<T>.DrawControls( this, VfxFile, Selected, Group, AllowNew, AllowDelete, parentId );
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
            if( Selected != null ) Selected.DrawInline( Id );
            else ImGui.Text( DefaultText );
        }

        public void ResetSelected() { Selected = null; }
    }
}
