using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxNodeDropdownView<T> : IUiItem, IUiNodeView<T> where T : AvfxNode {
        public readonly AvfxFile File;
        public readonly NodeGroup<T> Group;

        public readonly string Name;
        public readonly string DefaultText;
        public readonly string DefaultPath;
        private readonly bool AllowNew;
        private readonly bool AllowDelete;

        public T Selected = null;

        public AvfxNodeDropdownView( AvfxFile file, NodeGroup<T> group, string name, bool allowNew, bool allowDelete, string defaultPath ) {
            File = file;
            Group = group;
            AllowNew = allowNew;
            AllowDelete = allowDelete;

            Name = name;
            DefaultText = $"Select {UiUtils.GetArticle( name )} {name}";
            DefaultPath = Path.Combine( Plugin.RootLocation, "Files", defaultPath );
        }

        public abstract void OnSelect( T item );

        public abstract T Read( BinaryReader reader, int size );

        public void Draw() {
            using var _ = ImRaii.PushId( Name );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ViewSelect();

            if( AllowNew ) ImGui.SameLine();
            IUiNodeView<T>.DrawControls( this, File );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            Selected?.Draw();
        }

        public void ViewSelect() {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 2, 4 ) );

            var leftRightSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.ChevronLeft ) - 5;
            var inputSize = UiUtils.GetOffsetInputSize( leftRightSize * 2 );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                var index = Selected == null ? -1 : Group.Items.IndexOf( Selected );

                if( UiUtils.DisabledTransparentButton( FontAwesomeIcon.ChevronLeft.ToIconString(), new Vector4( 1 ), Selected != null && index > 0 ) ) {
                    Selected = Group.Items[index - 1];
                    OnSelect( Selected );
                }
                ImGui.SameLine();
                if( UiUtils.DisabledTransparentButton( FontAwesomeIcon.ChevronRight.ToIconString(), new Vector4( 1 ), Selected != null && index < ( Group.Items.Count - 1 ) ) ) {
                    Selected = Group.Items[index + 1];
                    OnSelect( Selected );
                }
            }

            ImGui.SameLine();
            ImGui.SetNextItemWidth( inputSize );

            var selectedString = ( Selected != null ) ? Selected.GetText() : DefaultText;
            using var combo = ImRaii.Combo( "##Select", selectedString );
            if( !combo ) return;

            for( var idx = 0; idx < Group.Items.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );

                var item = Group.Items[idx];
                if( ImGui.Selectable( item.GetText(), Selected == item ) ) {
                    Selected = item;
                    OnSelect( Selected );
                }
            }
        }

        public void ClearSelected() { Selected = null; }

        public NodeGroup<T> GetGroup() => Group;

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
