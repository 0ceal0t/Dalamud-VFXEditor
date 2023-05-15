using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Numerics;

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
            if( CurrentFile != null && ImGui.BeginMenu( "Edit" ) ) {
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

            var pos1 = ImGui.GetCursorScreenPos() + new Vector2( 0, -1 );
            ImGui.SameLine();
            var pos2 = ImGui.GetCursorScreenPos();
            var color = ImGui.GetColorU32( ImGuiCol.TabActive );
            var drawlist = ImGui.GetWindowDrawList();
            var offset = ( float )Math.Floor( ImGui.GetStyle().WindowPadding.X * 0.5f );
            drawlist.AddLine( pos1, new Vector2( pos2.X - ImGui.GetStyle().ItemSpacing.X + 4 - offset, pos1.Y ), color, 1 );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - ImGui.GetStyle().ItemSpacing.X + 4 );

            using var _ = ImRaii.PushId( "Tabs" );
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

                if( !open && Documents.Count > 1 ) ImGui.OpenPopup( $"DeletePopup" );

                if( ImGui.IsItemClicked( ImGuiMouseButton.Left ) && open ) SelectDocument( document );

                if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( $"RenamePopup" );

                using( var popup = ImRaii.Popup( "RenamePopup" ) ) {
                    if( popup ) document.DrawRename();
                }

                using( var popup = ImRaii.Popup( "DeletePopup" ) ) {
                    if( popup ) {
                        if( ImGui.Selectable( $"Delete" ) ) {
                            RemoveDocument( document );
                            break;
                        }
                    }
                }
            }

            if( ImGui.TabItemButton( $"+", ImGuiTabItemFlags.Trailing | ImGuiTabItemFlags.NoReorder | ImGuiTabItemFlags.NoTooltip ) ) AddDocument();
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
