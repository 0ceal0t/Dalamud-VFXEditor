using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxNodeSplitView<T> : AvfxGenericSplitView<T>, IUiNodeView<T> where T : AvfxNode {
        public readonly AvfxFile File;
        public readonly NodeGroup<T> Group;
        public readonly string DefaultPath;

        public AvfxNodeSplitView( AvfxFile file, NodeGroup<T> group, string name, bool allowNew, bool allowDelete, string defaultPath ) : base( name, group.Items, allowNew, allowDelete ) {
            File = file;
            Group = group;
            DefaultPath = Path.Combine( Plugin.RootLocation, "Files", defaultPath );
        }

        public abstract void OnSelect( T item );

        public abstract T Read( BinaryReader reader, int size );

        protected virtual bool IsDanger( T item ) => false;

        protected override void DrawControls() => IUiNodeView<T>.DrawControls( this, File );

        protected override bool DrawLeftItem( T item, int idx ) {
            using var _ = ImRaii.PushId( idx );
            using var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.RED_COLOR, IsDanger( item ) );

            if( ImGui.Selectable( item.GetText(), Selected == item ) ) {
                Selected = item;
                OnSelect( item );
            }
            return false;
        }

        protected override void DrawSelected() => Selected.Draw();

        public void ResetSelected() { Selected = null; }

        public NodeGroup<T> GetGroup() => Group;

        public string GetDefaultPath() => DefaultPath;

        public bool IsAllowedNew() => ShowControls;

        public bool IsAllowedDelete() => AllowDelete;

        public void SetSelected( T selected ) {
            Selected = selected;
            OnSelect( selected );
        }
    }
}
