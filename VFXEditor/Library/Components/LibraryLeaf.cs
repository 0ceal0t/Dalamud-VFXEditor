using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Library.Components {
    public abstract class LibraryLeaf : LibraryGeneric {
        protected Vector4 Color;

        private bool Editing = false;

        public LibraryLeaf( LibraryFolder parent, string name, string id, Vector4 color ) : base( parent, name, id ) {
            Color = color;
        }

        public string NameOrNone => string.IsNullOrEmpty( Name ) ? "[NONE]" : Name;

        public override bool Matches( string input ) {
            if( string.IsNullOrEmpty( input ) ) return true;
            if( string.IsNullOrEmpty( Name ) ) return false;
            if( Name.Contains( input, System.StringComparison.CurrentCultureIgnoreCase ) ) return true;
            return false;
        }

        public override bool Contains( LibraryGeneric item ) => false;

        protected void DrawHeader() {
            ImGui.PushStyleColor( ImGuiCol.Header, Color );
            ImGui.PushStyleColor( ImGuiCol.HeaderHovered, Color * 0.75f );
            ImGui.PushStyleColor( ImGuiCol.HeaderActive, Color * 0.75f );
            ImGui.TreeNodeEx( NameOrNone, ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.NoTreePushOnOpen );
            ImGui.PopStyleColor( 3 );
        }

        protected abstract void DrawBody();

        protected abstract void DrawTooltip();

        protected abstract void DrawEditing();

        protected abstract void DrawImport();

        public override bool Draw( LibraryManager library, string searchInput ) {
            using var _ = ImRaii.PushId( Id );

            var listModified = false;

            DrawHeader();
            DrawTooltip();
            DragDrop( library, Name, ref listModified );

            if( DrawPopup( library ) ) listModified = true;

            if( !Editing ) {
                DrawBody();
                return listModified;
            }

            // =========== Editing ===========

            using var indent = ImRaii.PushIndent();

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Save.ToIconString() ) ) {
                    Editing = false;
                    library.Save();
                    listModified = true;
                }
            }

            DrawEditing();

            return listModified;
        }

        private bool DrawPopup( LibraryManager library ) {
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "Popup" );

            using var popup = ImRaii.Popup( "Popup" );
            if( !popup ) return false;

            DrawImport();

            if( UiUtils.IconSelectable( FontAwesomeIcon.Edit, "Edit" ) ) {
                Editing = true;
            }

            if( UiUtils.IconSelectable( FontAwesomeIcon.Trash, "Delete" ) ) {
                Cleanup();
                Parent.Remove( this );
                library.Save();
                return true;
            }

            if( ImGui.InputText( "##Rename", ref Name, 128, ImGuiInputTextFlags.AutoSelectAll ) ) {
                library.Save();
                return true;
            }

            return false;
        }
    }
}
