using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System.Numerics;
using VfxEditor.Data.Command;
using VfxEditor.Data.Copy;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract partial class FileManager<D, F, S> : FileManagerBase where D : FileManagerDocument<F, S> where F : FileManagerFile {
        protected virtual void DrawEditMenuItems() { }

        public override void DrawBody() {
            if( ImGui.IsWindowFocused() ) Group.SetLastFocusedManager( this );

            using var copy = new CopyRaii( Copy );
            using var command = new CommandRaii( ActiveFile?.Command );

            CheckKeybinds();

            WindowSystem.Draw();
            WindowName = Title
                + ( string.IsNullOrEmpty( Plugin.CurrentWorkspaceName ) ? "" : $" [{Plugin.CurrentWorkspaceName}]" )
                + $"###{Title}-{WindowId}";

            using var _ = ImRaii.PushId( $"{FormatName}-{WindowId}" );
            DrawMenu();
            if( Plugin.Configuration.ShowTabBar ) {
                DrawTabs();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            }

            if( ActiveDocument == null ) { // No documents, make it clear how to add a new one
                ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( (ImGui.GetContentRegionAvail().X - 150f) / 2f, 25f) );
                if( ImGui.Button( "NEW", new( 150, 35 ) ) ) AddDocument();

                return;
            }

            ActiveDocument?.Draw();
        }

        private void DrawMenu() {
            var menu = ImGui.BeginMenuBar();
            if( !menu ) return;

            Plugin.DrawFileMenu( this, Group );

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

            using var smallItemSpacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 2 ) );

            var drawlist = ImGui.GetWindowDrawList();
            var color = ImGui.GetColorU32( ImGuiCol.TabActive );

            var preDropdownPos = ImGui.GetCursorScreenPos() + new Vector2( 0, -1 );
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 2 );
            var postDropdownPos = ImGui.GetCursorScreenPos();
            drawlist.AddLine( preDropdownPos, new Vector2( postDropdownPos.X, preDropdownPos.Y ), color, 1 );

            using var _ = ImRaii.PushId( "Tabs" );

            var size = ImGui.GetContentRegionAvail().X;
            var popupSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.ArrowUpRightFromSquare ) + UiUtils.GetPaddedIconSize( FontAwesomeIcon.PaintBrush );

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

                    if( Group.DrawDragDrop( this, document, document.DisplayName ) ) break;
                    if( !open ) ImGui.OpenPopup( "DeletePopup" );

                    if( ImGui.IsItemClicked( ImGuiMouseButton.Left ) && open ) SelectDocument( document );
                    if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "ContextPopup" );

                    using var itemSpacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 8, 4 ) );
                    using( var popup = ImRaii.Popup( "ContextPopup" ) ) {
                        if( popup ) {
                            if( ImGui.Selectable( "New Window" ) ) {
                                Group.ToNewWindow( this, document );
                                break;
                            }
                            document.DrawRename();
                        }
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
                if( Documents.Count == 0 ) Group.DrawDragDrop( this, null, null ); // in case the window has no documents
            }

            ImGui.SameLine();
            var prePopoutPos = ImGui.GetCursorScreenPos();
            using( var transparentButtonStyle = ImRaii.PushColor( ImGuiCol.Button, new Vector4( 0 ) ) )
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.ArrowUpRightFromSquare.ToIconString() ) ) DocumentWindow.Show();

                using( var buttonColor = ImRaii.PushColor( ImGuiCol.Text, WindowColor, UseWindowColor )) {
                    ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.PaintBrush.ToIconString() ) ) ImGui.OpenPopup( "WindowColorPicker" );
                }

                drawlist.AddLine( new Vector2( prePopoutPos.X, preDropdownPos.Y ), new Vector2( prePopoutPos.X + popupSize, preDropdownPos.Y ), color, 1 );
            }

            smallItemSpacing.Pop();

            using var windowPopup = ImRaii.Popup( "WindowColorPicker" );
            if( windowPopup ) {
                ImGui.Checkbox( "Use Custom Window Color", ref UseWindowColor );
                ImGui.ColorPicker4( "##Color", ref WindowColor, ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.AlphaBar );
            }
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

        public override unsafe void PreDraw() {
            base.PreDraw();

            ImGui.PushStyleColor( ImGuiCol.TitleBg, UseWindowColor ? WindowColor :
                ( Configuration.UseCustomWindowColor ? Configuration.TitleBg : *ImGui.GetStyleColorVec4(ImGuiCol.TitleBg) ) );

            ImGui.PushStyleColor( ImGuiCol.TitleBgActive, UseWindowColor ? WindowColor :
                ( Configuration.UseCustomWindowColor ? Configuration.TitleBgActive : *ImGui.GetStyleColorVec4( ImGuiCol.TitleBgActive ) ) );

            ImGui.PushStyleColor( ImGuiCol.TitleBgCollapsed, UseWindowColor ? WindowColor :
                ( Configuration.UseCustomWindowColor ? Configuration.TitleBgCollapsed : *ImGui.GetStyleColorVec4( ImGuiCol.TitleBgCollapsed ) ) );
        }

        public override void PostDraw() {
            base.PostDraw();
            ImGui.PopStyleColor( 3 );
        }
    }
}
