using ImGuiNET;
using System;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public abstract class UiNodeSplitView<T> : UiGenericSplitView, IUiNodeView<T> where T : AvfxNode {
        public readonly AvfxFile File;
        public readonly UiNodeGroup<T> Group;

        public readonly string Id;
        public readonly string DefaultText;
        public readonly string DefaultPath;

        public T Selected = null;

        public UiNodeSplitView( AvfxFile file, UiNodeGroup<T> group, string name, bool allowNew, bool allowDelete, string defaultPath ) : base( allowNew, allowDelete ) {
            File = file;
            Group = group;
            AllowNew = allowNew;
            AllowDelete = allowDelete;

            Id = $"##{name}";
            DefaultText = $"Select {UiUtils.GetArticle( name )} {name}";
            DefaultPath = Path.Combine( Plugin.RootLocation, "Files", defaultPath );
        }

        public abstract void OnSelect( T item );

        public abstract T Read( BinaryReader reader, int size );

        public override void DrawControls( string parentId ) => IUiNodeView<T>.DrawControls( this, File, parentId );

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
            if( Selected != null ) Selected.Draw( Id );
            else ImGui.Text( DefaultText );
        }

        public void ResetSelected() { Selected = null; }

        public UiNodeGroup<T> GetGroup() => Group;

        public string GetDefaultPath() => DefaultPath;

        public T GetSelected() => Selected;

        public bool IsAllowedNew() => AllowNew;

        public bool IsAllowedDelete() => AllowDelete;

        public void SetSelected( T selected ) {
            Selected = selected;
            OnSelect( selected );
        }
    }
}
