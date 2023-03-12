using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using ImPlotNET;
using System;
using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public abstract class UiNodeDropdownView<T> : IAvfxUiBase, IUiNodeView<T> where T : AvfxNode {
        public readonly AvfxFile File;
        public readonly UiNodeGroup<T> Group;

        public readonly string Id;
        public readonly string DefaultText;
        public readonly string DefaultPath;
        private readonly bool AllowNew;
        private readonly bool AllowDelete;

        public T Selected = null;

        public UiNodeDropdownView( AvfxFile file, UiNodeGroup<T> group, string name, bool allowNew, bool allowDelete, string defaultPath ) {
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

        public void Draw( string parentId = "" ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ViewSelect();

            if( AllowNew ) ImGui.SameLine();
            IUiNodeView<T>.DrawControls( this, File, parentId );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            Selected?.Draw( Id );
        }

        public void ViewSelect() {
            ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 2, 4 ) );

            var leftRightSize = UiUtils.GetIconSize( FontAwesomeIcon.ChevronLeft ) - 5;
            var inputSize = UiUtils.GetOffsetInputSize( leftRightSize * 2 );

            ImGui.PushFont( UiBuilder.IconFont );

            var index = Selected == null ? -1 : Group.Items.IndexOf( Selected );
            if( UiUtils.DisabledTransparentButton( $"{( char )FontAwesomeIcon.ChevronLeft}{Id}-Left", new Vector4( 1 ), Selected != null && index > 0 ) ) {
                Selected = Group.Items[index - 1];
                OnSelect( Selected );
            }
            ImGui.SameLine();
            if( UiUtils.DisabledTransparentButton( $"{( char )FontAwesomeIcon.ChevronRight}{Id}-Right", new Vector4( 1 ), Selected != null && index < ( Group.Items.Count - 1 ) ) ) {
                Selected = Group.Items[index + 1];
                OnSelect( Selected );
            }
            ImGui.PopFont();

            ImGui.SameLine();
            ImGui.SetNextItemWidth( inputSize );

            var selectedString = ( Selected != null ) ? Selected.GetText() : DefaultText;
            if( ImGui.BeginCombo( Id + "-Select", selectedString ) ) {
                for( var idx = 0; idx < Group.Items.Count; idx++ ) {
                    var item = Group.Items[idx];
                    if( ImGui.Selectable( $"{item.GetText()}{Id}{idx}", Selected == item ) ) {
                        Selected = item;
                        OnSelect( Selected );
                    }
                }
                ImGui.EndCombo();
            }

            ImGui.PopStyleVar( 1 );
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
