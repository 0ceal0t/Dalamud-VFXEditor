using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract partial class FileManagerWindow<T, R, S> : FileManagerWindow, IFileManager where T : FileManagerDocument<R, S> where R : FileManagerFile {
        protected virtual void DrawEditMenuExtra() { }

        public override void DrawBody() {
            SourceSelect?.Draw();
            ReplaceSelect?.Draw();

            Name = WindowTitle
                + ( string.IsNullOrEmpty( Plugin.CurrentWorkspaceLocation ) ? "" : " - " + Plugin.CurrentWorkspaceLocation )
                + "###"
                + WindowTitle;

            CheckKeybinds();

            DocumentWindow.Draw();

            using var _ = ImRaii.PushId( $"##{Id}" );
            DrawMenu();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() - ( ImGui.GetStyle().WindowPadding.Y - 4 ) );
            DrawTabs();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ActiveDocument?.Draw();
        }

        private void DrawMenu() {
            var menu = ImGui.BeginMenuBar();
            if( !menu ) return;

            Plugin.DrawFileMenu();

            if( CurrentFile == null ) {
                using var disabled = ImRaii.Disabled();
                ImGui.MenuItem( "Edit" );
            }
            else if( ImGui.BeginMenu( "Edit" ) ) {
                GetCopyManager().Draw();
                GetCommandManager().Draw();
                DrawEditMenuExtra();
                ImGui.EndMenu();
            }

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
                using var tabBar = ImRaii.TabBar( "", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton | ImGuiTabBarFlags.Reorderable );
                if( !tabBar ) return;

                for( var i = 0; i < Documents.Count; i++ ) {
                    using var __ = ImRaii.PushId( i );
                    var document = Documents[i];

                    var open = true;
                    var flags = ImGuiTabItemFlags.None | ImGuiTabItemFlags.NoPushId;
                    if( ActiveDocument == document ) flags |= ImGuiTabItemFlags.SetSelected;
                    if( document.Unsaved ) flags |= ImGuiTabItemFlags.UnsavedDocument;

                    if( ImGui.BeginTabItem( $"{document.DisplayName}###", ref open, flags ) ) ImGui.EndTabItem();

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

                if( ImGui.TabItemButton( $"+", ImGuiTabItemFlags.Trailing | ImGuiTabItemFlags.NoReorder | ImGuiTabItemFlags.NoTooltip ) ) AddDocument();
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

        protected override void PreDraw() {
            if( !Configuration.UseCustomWindowColor ) return;
            ImGui.PushStyleColor( ImGuiCol.TitleBg, Configuration.TitleBg );
            ImGui.PushStyleColor( ImGuiCol.TitleBgActive, Configuration.TitleBgActive );
            ImGui.PushStyleColor( ImGuiCol.TitleBgCollapsed, Configuration.TitleBgCollapsed );
        }

        protected override void PostDraw() {
            if( !Configuration.UseCustomWindowColor ) return;
            ImGui.PopStyleColor( 3 );
        }
    }
}
