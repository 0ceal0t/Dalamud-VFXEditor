using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using System.Numerics;
using VfxEditor.FilePicker.FolderFiles;

namespace VfxEditor.FilePicker {
    public partial class FilePickerDialog {
        private bool SideBarDrawn = false;

        private void DrawBody() {
            var size = ImGui.GetContentRegionAvail() - new Vector2( 0, ImGui.GetFrameHeightWithSpacing() );

            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) ) )
            using( var __ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 0 ) ) ) {
                ImGui.BeginChild( "Child", size );
                ImGui.Columns( 2, "Columns" );
            }

            if( !SideBarDrawn ) {
                SideBarDrawn = true;
                ImGui.SetColumnWidth( 0, 150 );
            }
            SideBar.Draw();

            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) ) )
            using( var __ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 0 ) ) ) {
                ImGui.NextColumn();
            }

            DrawFileList();

            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) ) )
            using( var __ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 0 ) ) ) {
                ImGui.Columns( 1 );
                ImGui.EndChild();
            }
        }

        private void DrawFileList() {
            using var _ = ImRaii.PushId( "Files" );

            using var child = ImRaii.Child( "Child" );

            using var table = ImRaii.Table( "Table", 4,
                ImGuiTableFlags.SizingFixedFit |
                ImGuiTableFlags.RowBg |
                ImGuiTableFlags.Hideable |
                ImGuiTableFlags.ScrollY |
                ImGuiTableFlags.NoHostExtendX );
            if( !table ) return;

            ImGui.TableSetupScrollFreeze( 0, 1 );

            ImGui.TableSetupColumn( "  Name", ImGuiTableColumnFlags.WidthStretch, -1, 0 );
            ImGui.TableSetupColumn( "Type", ImGuiTableColumnFlags.WidthFixed, -1, 1 );
            ImGui.TableSetupColumn( "Size", ImGuiTableColumnFlags.WidthFixed, -1, 2 );
            ImGui.TableSetupColumn( "Date  ", ImGuiTableColumnFlags.WidthFixed, -1, 3 );

            ImGui.TableNextRow( ImGuiTableRowFlags.Headers );
            for( var column = 0; column < 4; column++ ) {
                ImGui.TableSetColumnIndex( column );
                var columnName = ImGui.TableGetColumnName( column );
                using( var __ = ImRaii.PushId( column ) ) {
                    ImGui.TableHeader( columnName );
                }

                if( ImGui.IsItemClicked() ) SortFiles( ( SortingField )( column + 1 ), true );
            }

            if( SearchedFiles.Count == 0 ) return;

            ImGuiListClipperPtr clipper;
            unsafe {
                clipper = new ImGuiListClipperPtr( ImGuiNative.ImGuiListClipper_ImGuiListClipper() );
            }

            clipper.Begin( SearchedFiles.Count );

            while( clipper.Step() ) {
                for( var idx = clipper.DisplayStart; idx < clipper.DisplayEnd; idx++ ) {
                    if( idx < 0 || idx >= SearchedFiles.Count ) continue;

                    var file = SearchedFiles[idx];
                    using var __ = ImRaii.PushId( idx );

                    if( file.Draw( file == Selected, out var doubleClicked ) ) {
                        if( file.Type == FilePickerFileType.Directory ) {
                            if( doubleClicked ) {
                                if( NavigateTo( file ) ) break;
                            }
                            else if( FolderDialog ) {
                                Selected = file;
                                FileNameInput = file.FileName;
                            }
                        }
                        else {
                            Selected = file;
                            FileNameInput = file.FileName;
                            if( doubleClicked ) {
                                WantsToQuit = true;
                                IsOk = true;
                            }
                        }
                    }
                }
            }

            clipper.End();
        }

        private bool NavigateTo( FilePickerFile file ) {
            if( file.FileName == ".." ) {
                if( PathParts.Count > 1 ) {
                    SetPath( ComposeNewPath( PathParts.GetRange( 0, PathParts.Count - 1 ) ) );
                    return true;
                }
            }
            else {
                var newPath = Path.Combine( CurrentPath, file.FileName );
                if( Directory.Exists( newPath ) ) {
                    SetPath( newPath );
                    return true;
                }
            }

            return false;
        }
    }
}
