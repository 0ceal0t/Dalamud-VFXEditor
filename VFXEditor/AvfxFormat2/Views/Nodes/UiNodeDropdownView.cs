using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat2 {
    public abstract class UiNodeDropdownView<T> : IUiBase, IUiNodeView<T> where T : AvfxNode {
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

        public void Draw( string parentId = "" ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ViewSelect();

            if( AllowNew ) ImGui.SameLine();
            IUiNodeView<T>.DrawControls( this, File, parentId );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 10 );

            if( Selected != null ) Selected.Draw( Id );
        }

        public void ViewSelect() {
            var selectedString = ( Selected != null ) ? Selected.GetText() : DefaultText;
            if( ImGui.BeginCombo( Id + "-Select", selectedString ) ) {
                for( var idx = 0; idx < Group.Items.Count; idx++ ) {
                    var item = Group.Items[idx];
                    if( ImGui.Selectable( $"{item.GetText()}{Id}{idx}", Selected == item ) ) {
                        Selected = item;
                        OnSelect( item );
                    }
                }
                ImGui.EndCombo();
            }
        }

        public void ResetSelected() { Selected = null; }

        public UiNodeGroup<T> GetGroup() => Group;

        public string GetDefaultPath() => DefaultPath;

        public T GetSelected() => Selected;

        public bool IsAllowedNew() => AllowNew;

        public bool IsAllowedDelete() => AllowDelete;
    }
}
