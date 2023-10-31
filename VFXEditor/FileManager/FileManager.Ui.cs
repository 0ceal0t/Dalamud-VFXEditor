using Dalamud.Interface;
using ImGuiNET;
using OtterGui;
using OtterGui.Raii;
using System.Numerics;
using VfxEditor.Data.Command;
using VfxEditor.Data.Copy;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract partial class FileManager<T, R, S> : FileManagerBase where T : FileManagerDocument<R, S> where R : FileManagerFile {
        private T DraggingItem;

        protected virtual void DrawEditMenuItems() { }

        public override void DrawBody() {
            using var copy = new CopyRaii( Copy );
            using var command = new CommandRaii( File?.Command );

            CheckKeybinds();

            WindowSystem.Draw();
            WindowName = Title
                + ( string.IsNullOrEmpty( Plugin.CurrentWorkspaceName ) ? "" : $" [{Plugin.CurrentWorkspaceName}]" )
                + $"###{Title}";

            using var _ = ImRaii.PushId( Id );
            DrawMenu();
            if( Plugin.Configuration.ShowTabBar ) {
                DrawTabs();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            }

            ActiveDocument?.Draw();
        }

        private void DrawMenu() {
            var menu = ImGui.BeginMenuBar();
            if( !menu ) return;

            Plugin.DrawFileMenu();

            if( ImGui.BeginMenu( "Edit" ) ) {
                CommandManager.Draw();
                CopyManager.Draw();
                DrawEditMenuItems();
                ImGui.EndMenu();
            }

            if( !Plugin.Configuration.ShowTabBar && ImGui.MenuItem( "Documents" ) ) DocumentWindow.Show();

            ImGui.Separator();
            Plugin.DrawManagersMenu( this );

            ImGui.EndMenuBar();
        }

        private void DrawTabs() {
            DrawTabsDropdown();

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 2 ) );

            var drawlist = ImGui.GetWindowDrawList();
            var color = ImGui.GetColorU32( ImGuiCol.TabActive );

            var preDropdownPos = ImGui.GetCursorScreenPos() + new Vector2( 0, -1 );
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 2 );
            var postDropdownPos = ImGui.GetCursorScreenPos();
            drawlist.AddLine( preDropdownPos, new Vector2( postDropdownPos.X, preDropdownPos.Y ), color, 1 );

            using var _ = ImRaii.PushId( "Tabs" );

            var size = ImGui.GetContentRegionAvail().X;
            var popupSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.ArrowUpRightFromSquare ) - ImGui.GetStyle().ItemInnerSpacing.X;

            using( var child = ImRaii.Child( "Child", new Vector2( size - popupSize, ImGui.GetFrameHeightWithSpacing() ) ) ) {
                using var tabs = ImRaii.TabBar( "TabBar", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
                if( !tabs ) return;

                foreach( var (document, idx) in Documents.WithIndex() ) {
                    using var __ = ImRaii.PushId( idx );

                    var open = true;
                    var flags = ImGuiTabItemFlags.NoTooltip;
                    if( ActiveDocument == document ) flags |= ImGuiTabItemFlags.SetSelected;
                    if( document.Unsaved ) flags |= ImGuiTabItemFlags.UnsavedDocument;

                    if( ImGui.BeginTabItem( $"{document.DisplayName}###Tab{idx}", ref open, flags ) ) ImGui.EndTabItem();

                    if( UiUtils.DrawDragDrop( Documents, document, document.DisplayName, ref DraggingItem, "DOCUMENT-TABS", false ) ) break;

                    if( !open && Documents.Count > 1 ) ImGui.OpenPopup( "DeletePopup" );

                    if( ImGui.IsItemClicked( ImGuiMouseButton.Left ) && open ) SelectDocument( document );
                    if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "RenamePopup" );

                    using var itemSpacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 8, 4 ) );
                    using( var popup = ImRaii.Popup( "RenamePopup" ) ) {
                        if( popup ) document.DrawRename();
                    }
                    using( var popup = ImRaii.Popup( "DeletePopup" ) ) {
                        if( popup ) {
                            if( UiUtils.IconSelectable( FontAwesomeIcon.Trash, "Delete" ) ) {
                                RemoveDocument( document );
                                break;
                            }
                        }
                    }
                }

                if( ImGui.TabItemButton( "+", ImGuiTabItemFlags.Trailing | ImGuiTabItemFlags.NoReorder | ImGuiTabItemFlags.NoTooltip ) ) AddDocument();
            }

            ImGui.SameLine();
            var prePopoutPos = ImGui.GetCursorScreenPos();
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            using var transparentButtonStyle = ImRaii.PushColor( ImGuiCol.Button, new Vector4( 0 ) );
            if( ImGui.Button( FontAwesomeIcon.ArrowUpRightFromSquare.ToIconString() ) ) DocumentWindow.Show();
            drawlist.AddLine( new Vector2( prePopoutPos.X, preDropdownPos.Y ), new Vector2( prePopoutPos.X + popupSize, preDropdownPos.Y ), color, 1 );
        }

        private void DrawTabsDropdown() {
            using var _ = ImRaii.PushId( "Combo" );

            using var color = ImRaii.PushColor( ImGuiCol.Button, new Vector4( 0 ) );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 1, 0 ) );
            using var combo = ImRaii.Combo( "", "", ImGuiComboFlags.NoPreview );
            style.Pop();
            color.Pop();
            if( !combo ) return;

            for( var i = 0; i < Documents.Count; i++ ) {
                using var __ = ImRaii.PushId( i );
                var document = Documents[i];
                if( ImGui.Selectable( document.DisplayName, document == ActiveDocument ) ) SelectDocument( document );
            }
        }

        public override void PreDraw() {
            base.PreDraw();
            if( !Configuration.UseCustomWindowColor ) return;
            ImGui.PushStyleColor( ImGuiCol.TitleBg, Configuration.TitleBg );
            ImGui.PushStyleColor( ImGuiCol.TitleBgActive, Configuration.TitleBgActive );
            ImGui.PushStyleColor( ImGuiCol.TitleBgCollapsed, Configuration.TitleBgCollapsed );
        }

        public override void PostDraw() {
            if( !Configuration.UseCustomWindowColor ) return;
            ImGui.PopStyleColor( 3 );
        }
    }
}
