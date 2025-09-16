using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.FileBrowser {
    public partial class FileBrowserDialog {
        private void DrawHeader() {
            DrawPathComposer();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );

            DrawSearchBar();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 0 ) );
            ImGui.Separator();
        }

        private unsafe void DrawPathComposer() {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( PathEditing ? UiUtils.RemoveButton( FontAwesomeIcon.Times.ToIconString() ) : ImGui.Button( FontAwesomeIcon.Search.ToIconString() ) ) PathEditing = !PathEditing;
            }

            if( PathParts.Count == 0 ) return;

            if( PathEditing ) {
                ImGui.SameLine();
                ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X );
                ImGui.InputText( "##PathEdit", ref PathInput, 255 );

                if( ImGui.IsKeyReleased( ImGuiKey.Enter ) ) {
                    if( Directory.Exists( PathInput ) ) SetPath( PathInput, true );
                    PathEditing = false;
                }
                if( ImGui.IsKeyReleased( ImGuiKey.Escape ) ) PathEditing = false;

                return;
            }

            using var color = ImRaii.PushColor( ImGuiCol.Button, *ImGui.GetStyleColorVec4( ImGuiCol.TableHeaderBg ) );

            foreach( var (part, idx) in PathParts.WithIndex() ) {
                ImGui.SameLine();

                using var _ = ImRaii.PushId( idx );
                if( ImGui.Button( part ) ) {
                    SetPath( ComposeNewPath( PathParts.GetRange( 0, idx + 1 ) ), true );
                    return;
                }

                if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) {
                    PathInput = ComposeNewPath( PathParts.GetRange( 0, idx + 1 ) );
                    PathEditing = true;
                    return;
                }
            }
        }

        private void DrawSearchBar() {
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) )
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                using( var disabled = ImRaii.Disabled( !PathHistory.CanUndo ) ) {
                    if( ImGui.Button( FontAwesomeIcon.ArrowLeft.ToIconString() ) && PathHistory.Undo( out var path ) ) SetPath( path.Item1, false );
                }

                using( var disabled = ImRaii.Disabled( !PathHistory.CanRedo ) ) {
                    ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.ArrowRight.ToIconString() ) && PathHistory.Redo( out var path ) ) SetPath( path.Item2, false );
                }

                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Home.ToIconString() ) ) SetPath( ".", true );

                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.FolderPlus.ToIconString() ) && !CreatingFolder ) {
                    CreatingFolder = true;
                    NewFolderInput = "";
                }

                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Sync.ToIconString() ) ) UpdateFiles();

                ImGui.SameLine();
                var isFavorite = Plugin.Configuration.IsFileBrowserFavorite( CurrentPath );
                using( var star = ImRaii.PushColor( ImGuiCol.Text, UiUtils.DALAMUD_YELLOW, isFavorite ) ) {
                    if( ImGui.Button( FontAwesomeIcon.Star.ToIconString() ) ) {
                        if( isFavorite ) Plugin.Configuration.RemoveFileBrowserFavorite( CurrentPath );
                        else Plugin.Configuration.AddFileBrowserFavorite( CurrentPath );
                    }
                }
            }

            ImGui.SameLine();
            var space = ImGui.GetContentRegionAvail().X;
            if( space > 400 ) {
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() + space - 400 );
            }

            if( CreatingFolder ) {
                DrawDirectoryCreation();
            }
            else {
                ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X );
                if( ImGui.InputTextWithHint( "##Search", "Search", ref SearchInput, 255 ) ) UpdateSearchedFiles();
            }
        }

        private void DrawDirectoryCreation() {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Check );
            var cancelSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Times );

            ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X - checkSize - cancelSize );
            ImGui.InputTextWithHint( "##NewFolder", "New Folder Name", ref NewFolderInput, 255 );

            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            ImGui.SameLine();
            if( ImGui.Button( FontAwesomeIcon.Check.ToIconString() ) ) {
                if( CreateDirectory( NewFolderInput ) ) {
                    SetPath( Path.Combine( CurrentPath, NewFolderInput ), true );
                }
                CreatingFolder = false;
            }

            ImGui.SameLine();
            if( UiUtils.RemoveButton( FontAwesomeIcon.Times.ToIconString() ) ) {
                CreatingFolder = false;
            }
        }

        private bool CreateDirectory( string dir ) {
            var path = Path.Combine( CurrentPath, dir );
            if( string.IsNullOrEmpty( path ) ) return false;

            Directory.CreateDirectory( path );
            return true;
        }
    }
}
