using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FDialog {
    public partial class FileDialog {
        public bool Draw() {
            if( !Visible ) return false;

            var res = false;
            var name = Title + "##" + Id;

            bool windowVisible;
            IsOk = false;
            WantsToQuit = false;

            ResetEvents();

            // TOOD: size

            if( IsModal && !OkResultToConfirm ) {
                ImGui.OpenPopup( name );
                windowVisible = ImGui.BeginPopupModal( name, ref Visible, ImGuiWindowFlags.NoScrollbar );
            }
            else {
                windowVisible = ImGui.Begin( name, ref Visible, ImGuiWindowFlags.NoScrollbar );
            }

            if( windowVisible ) {
                AnyWindowsHovered = ImGui.IsWindowHovered();

                if( SelectedFilter.Empty() && ( Filters.Count > 0 ) ) {
                    SelectedFilter = Filters[0];
                }

                if( Files.Count == 0 && !ShowDrives ) {
                    if( !string.IsNullOrEmpty( DefaultFileName ) ) {
                        SetDefaultFileName( DefaultFileName );
                        SetSelectedFilterWithExt( DefaultExtension );
                    }
                    else if( IsDirectoryMode() ) {
                        SetDefaultFileName( "." );
                    }

                    ScanDir( CurrentPath );
                }

                DrawHeader();
                DrawContent();
                res = DrawFooter();

                // TODO: center position

                if( IsModal && !OkResultToConfirm ) {
                    ImGui.EndPopup();
                }
            }

            if( !IsModal || OkResultToConfirm ) {
                ImGui.End();
            }

            return ConfirmOrOpenOverWriteFileDialogIfNeeded( res );
        }

        private void DrawHeader() {
            // TODO: bookmark

            DrawDirectoryCreation();
            DrawPathComposer();
            DrawSearchBar();
        }

        private void DrawDirectoryCreation() {
            if( ( Flags & ImGuiFileDialogFlags.DisableCreateDirectoryButton ) != ImGuiFileDialogFlags.DisableCreateDirectoryButton ) return;

            if( ImGui.Button( "+" ) ) {
                if( !CreateDirectoryMode ) {
                    CreateDirectoryMode = true;
                    CreateDirectoryBuffer = "";
                }
            }
            if( ImGui.IsItemHovered() ) {
                ImGui.SetTooltip( "Create Directory" );
            }

            if( CreateDirectoryMode ) {
                ImGui.SameLine();
                ImGui.PushItemWidth( 100f );
                ImGui.InputText( "##DirectoryFileName", ref CreateDirectoryBuffer, 255 );
                ImGui.PopItemWidth();

                ImGui.SameLine();

                if( ImGui.Button( "Ok" ) ) {
                    if( CreateDir( CreateDirectoryBuffer ) ) {
                        SetPath( Path.Combine( CurrentPath, CreateDirectoryBuffer ) );
                    }
                    CreateDirectoryMode = false;
                }

                ImGui.SameLine();

                if( ImGui.Button( "Cancel" ) ) {
                    CreateDirectoryMode = false;
                }
            }

            // TODO: vertical separator
        }

        private void DrawPathComposer() {
            if( ImGui.Button( "R" ) ) {
                SetPath( "." );
            }
            if( ImGui.IsItemHovered() ) {
                ImGui.SetTooltip( "Reset to current directory" );
            }

            ImGui.SameLine();

            if( ImGui.Button( "Drives" ) ) {
                DrivesClicked = true;
            }

            ImGui.SameLine();

            // TODO: vertical separator

            if( PathDecomposition.Count > 0 ) {
                ImGui.SameLine();

                if( PathInputActivated ) {
                    ImGui.PushItemWidth( ImGui.GetContentRegionAvail().X );
                    ImGui.InputText( "##pathedit", ref PathInputBuffer, 255 );
                    ImGui.PopItemWidth();
                }
                else {
                    for( int idx = 0; idx < PathDecomposition.Count; idx++ ) {
                        if( idx > 0 ) ImGui.SameLine();

                        ImGui.PushID( idx );
                        var click = ImGui.Button( PathDecomposition[idx] );
                        ImGui.PopID();

                        if( click ) {
                            CurrentPath = ComposeNewPath( PathDecomposition.GetRange( 0, idx + 1 ) );
                            PluginLog.Log( CurrentPath );
                            PathClicked = true;
                            break;
                        }

                        if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) {
                            PathInputBuffer = ComposeNewPath( PathDecomposition.GetRange( 0, idx + 1 ) );
                            PathInputActivated = true;
                            break;
                        }
                    }
                }
            }
        }

        private void DrawSearchBar() {
            ImGui.Text( "Search :" );
            ImGui.SameLine();
            ImGui.PushItemWidth( ImGui.GetContentRegionAvail().X );
            var edited = ImGui.InputText( "##InputImGuiFileDialogSearchField", ref SearchBuffer, 255 );
            ImGui.PopItemWidth();
            if( edited ) {
                SearchTag = SearchBuffer;
                ApplyFilteringOnFileList();
            }
        }

        private void DrawContent() {
            var size = ImGui.GetContentRegionAvail() - new Vector2( 0, FooterHeight );

            // TODO: bookmark

            // TODO: side panel

            DrawFileListView( size );
        }

        private unsafe void DrawFileListView(Vector2 size) {
            ImGui.BeginChild( "##FileDialog_FileList", size );

            ImGuiTableFlags tableFlags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg | ImGuiTableFlags.Hideable | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoHostExtendX;
            if(ImGui.BeginTable("##FileTable", 4, tableFlags, size)) {
                ImGui.TableSetupScrollFreeze( 0, 1 );

                var hideType = ( Flags & ImGuiFileDialogFlags.HideColumnType ) == ImGuiFileDialogFlags.HideColumnType;
                var hideSize = ( Flags & ImGuiFileDialogFlags.HideColumnSize ) == ImGuiFileDialogFlags.HideColumnSize;
                var hideDate = ( Flags & ImGuiFileDialogFlags.HideColumnDate ) == ImGuiFileDialogFlags.HideColumnDate;

                ImGui.TableSetupColumn( "File Name", ImGuiTableColumnFlags.WidthStretch, -1, 0 );
                ImGui.TableSetupColumn( "Type", ImGuiTableColumnFlags.WidthFixed | (hideType ? ImGuiTableColumnFlags.DefaultHide : ImGuiTableColumnFlags.None), -1, 1 );
                ImGui.TableSetupColumn( "Size", ImGuiTableColumnFlags.WidthFixed | ( hideSize ? ImGuiTableColumnFlags.DefaultHide : ImGuiTableColumnFlags.None ), -1, 2 );
                ImGui.TableSetupColumn( "Date", ImGuiTableColumnFlags.WidthFixed | ( hideDate ? ImGuiTableColumnFlags.DefaultHide : ImGuiTableColumnFlags.None ), -1, 3 );

                ImGui.TableNextRow( ImGuiTableRowFlags.Headers );
                for(int column = 0; column < 4; column++ ) {
                    ImGui.TableSetColumnIndex( column );
                    var columnName = ImGui.TableGetColumnName( column );
                    ImGui.PushID( column );
                    ImGui.TableHeader( columnName );
                    ImGui.PopID();
                    if( ImGui.IsItemClicked() ) {
                        if(column == 0) {
                            SortFields( SortingField.FileName, true );
                        }
                        else if(column == 1) {
                            SortFields( SortingField.Type, true );
                        }
                        else if(column == 2 ) {
                            SortFields( SortingField.Size, true );
                        }
                        else {
                            SortFields( SortingField.Date, true );
                        }
                    }
                }

                if(FilteredFiles.Count > 0 ) {
                    ImGuiListClipperPtr clipper;
                    unsafe {
                        clipper = new ImGuiListClipperPtr( ImGuiNative.ImGuiListClipper_ImGuiListClipper() );
                    }

                    lock(FilesLock) {
                        clipper.Begin( FilteredFiles.Count );
                        while( clipper.Step() ) {
                            for( var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++ ) {
                                if( i < 0 ) continue;

                                var file = FilteredFiles[i];

                                // TODO: icon
                                // TODO: color

                                string name = file.Type switch
                                {
                                    FileStructType.Directory => "[DIR] " + file.FileName,
                                    FileStructType.File => "[FILE] " + file.FileName,
                                    _ => file.FileName
                                };

                                var selected = SelectedFileNames.Contains( file.FileName );
                                var needToBreak = false;

                                ImGui.TableNextRow();

                                if(ImGui.TableNextColumn()) {
                                    needToBreak = SelectableItem( i, file, selected, name );
                                }
                                if(ImGui.TableNextColumn()) {
                                    ImGui.Text( file.Ext );
                                }
                                if( ImGui.TableNextColumn() ) {
                                    if(file.Type == FileStructType.File) {
                                        ImGui.Text( file.FormattedFileSize + " " );
                                    }
                                    else {
                                        ImGui.Text( " " );
                                    }
                                }
                                if( ImGui.TableNextColumn() ) {
                                    ImGui.Text( file.FileModifiedDate );
                                }

                                if( needToBreak ) break;
                            }
                        }

                        clipper.End();
                    }
                }

                if(PathInputActivated) {
                    if(ImGui.IsKeyReleased(ImGui.GetKeyIndex(ImGuiKey.Enter))) {
                        SetPath( PathInputBuffer );
                        PathInputActivated = false;
                    }
                    if(ImGui.IsKeyReleased(ImGui.GetKeyIndex(ImGuiKey.Escape))) {
                        PathInputActivated = false;
                    }
                }

                // TODO: key exploration

                ImGui.EndTable();
            }

            if(PathClicked) {
                SetPath( CurrentPath );
            }
            if(DrivesClicked) {
                GetDrives();
            }

            ImGui.EndChild();
        }

        private bool SelectableItem(int idx, FileStruct file, bool selected, string text) {
            var flags = ImGuiSelectableFlags.AllowDoubleClick | ImGuiSelectableFlags.SpanAllColumns;

            // TODO: key exploration

            var res = ImGui.Selectable( text, selected, flags );
            if(res) {
                if(file.Type == FileStructType.Directory) {
                    if(ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left)) {
                        PathClicked = SelectDirectory( file );
                        return true;
                    }
                    else if(IsDirectoryMode()) {
                        SelectFileName( file );
                    }
                }
                else {
                    SelectFileName( file );
                    if(ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left)) {
                        WantsToQuit = true;
                        IsOk = true;
                    }
                }
            }

            return false;
        }


        private bool SelectDirectory( FileStruct file ) {
            var pathClick = false;

            if( file.FileName == ".." ) {
                if( PathDecomposition.Count > 1 ) {
                    CurrentPath = ComposeNewPath( PathDecomposition.GetRange( 0, PathDecomposition.Count - 1 ) );
                    pathClick = true;
                }
            }
            else {
                string newPath;
                if( ShowDrives ) {
                    newPath = file.FileName;
                }
                else {
                    newPath = Path.Combine( CurrentPath, file.FileName );
                }

                if( Directory.Exists( newPath ) ) {
                    CurrentPath = newPath;
                }
                pathClick = true;
            }

            return pathClick;
        }

        private void SelectFileName( FileStruct file ) {
            if( ImGui.GetIO().KeyCtrl ) {
                if(SelectionCountMax == 0) {
                    if( SelectedFileNames.Contains( file.FileName ) ) {
                        AddFileNameInSelection( file.FileName, true );
                    }
                    else {
                        RemoveFileNameInSelection( file.FileName );
                    }
                }
                else {
                    if(SelectedFileNames.Count < SelectionCountMax) {
                        if( SelectedFileNames.Contains( file.FileName ) ) {
                            AddFileNameInSelection( file.FileName, true );
                        }
                        else {
                            RemoveFileNameInSelection( file.FileName );
                        }
                    }
                }
            }
            else if( ImGui.GetIO().KeyShift ) {
                if(SelectionCountMax != 1) {
                    SelectedFileNames.Clear();

                    var startMultiSelection = false;
                    var fileNameToSelect = file.FileName;
                    string savedLastSelectedFileName = "";

                    foreach(var f in Files) {
                        var canTake = true;
                        if( !string.IsNullOrEmpty( SearchTag ) && !f.FileName.Contains( SearchTag ) ) canTake = false; // filtered out
                        if(canTake) {
                            if( f.FileName == LastSelectedFileName) {
                                startMultiSelection = true;
                                AddFileNameInSelection( LastSelectedFileName, false );
                            }
                            else if(startMultiSelection) {
                                if(SelectionCountMax == 0) {
                                    AddFileNameInSelection( f.FileName, false );
                                }
                                else {
                                    if(SelectedFileNames.Count < SelectionCountMax) {
                                        AddFileNameInSelection( f.FileName, false );
                                    }
                                    else {
                                        startMultiSelection = false;
                                        if(!string.IsNullOrEmpty(savedLastSelectedFileName)) {
                                            LastSelectedFileName = savedLastSelectedFileName;
                                        }
                                        break;
                                    }
                                }
                            }

                            if(f.FileName == fileNameToSelect) {
                                if(!startMultiSelection) {
                                    savedLastSelectedFileName = LastSelectedFileName;
                                    LastSelectedFileName = fileNameToSelect;
                                    fileNameToSelect = savedLastSelectedFileName;
                                    startMultiSelection = true;
                                    AddFileNameInSelection( LastSelectedFileName, false );
                                }
                                else {
                                    startMultiSelection = false;
                                    if(!string.IsNullOrEmpty(savedLastSelectedFileName)) {
                                        LastSelectedFileName = savedLastSelectedFileName;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else {
                SelectedFileNames.Clear();
                FileNameBuffer = "";
                AddFileNameInSelection( file.FileName, true );
            }
        }

        private void AddFileNameInSelection(string name, bool setLastSelection) {
            SelectedFileNames.Add( name );
            if(SelectedFileNames.Count == 1) {
                FileNameBuffer = name;
            }
            else {
                FileNameBuffer = $"{SelectedFileNames.Count} files Selected";
            }

            if(setLastSelection) {
                LastSelectedFileName = name;
            }
        }

        private void RemoveFileNameInSelection(string name) {
            SelectedFileNames.Remove( name );
            if( SelectedFileNames.Count == 1 ) {
                FileNameBuffer = name;
            }
            else {
                FileNameBuffer = $"{SelectedFileNames.Count} files Selected";
            }
        }

        private bool DrawFooter() {
            var posY = ImGui.GetCursorPosY();

            if( IsDirectoryMode() ) {
                ImGui.Text( "Directory Path :" );
            }
            else {
                ImGui.Text( "File Name :" );
            }

            ImGui.SameLine();

            var width = ImGui.GetContentRegionAvail().X;
            if( Filters.Count > 0 ) {
                width -= 150f;
            }
            ImGui.PushItemWidth( width );
            ImGui.InputText( "##FileName", ref FileNameBuffer, 255 );
            ImGui.PopItemWidth();

            if( Filters.Count > 0 ) {
                ImGui.SameLine();
                var needToApplyNewFilter = false;

                ImGui.PushItemWidth( 150f );
                if( ImGui.BeginCombo( "##Filters", SelectedFilter.Filter, ImGuiComboFlags.None ) ) {
                    int idx = 0;
                    foreach( var filter in Filters ) {
                        var selected = ( filter.Filter == SelectedFilter.Filter );
                        ImGui.PushID( idx++ );
                        if( ImGui.Selectable( filter.Filter, selected ) ) {
                            SelectedFilter = filter;
                            needToApplyNewFilter = true;
                        }
                        ImGui.PopID();
                    }
                    ImGui.EndCombo();
                }
                ImGui.PopItemWidth();

                if( needToApplyNewFilter ) {
                    SetPath( CurrentPath );
                }
            }

            var res = false;

            if( CanWeContinue && !string.IsNullOrEmpty( FileNameBuffer ) ) {
                if( ImGui.Button( "Ok" ) ) {
                    IsOk = true;
                    res = true;
                }
                ImGui.SameLine();
            }

            if( ImGui.Button( "Cancel" ) ) {
                IsOk = false;
                res = true;
            }

            FooterHeight = ImGui.GetCursorPosY() - posY;

            if( WantsToQuit && IsOk ) {
                res = true;
            }

            return res;
        }

        private bool ConfirmOrOpenOverWriteFileDialogIfNeeded( bool lastAction ) {
            if( !IsOk && lastAction ) {
                return true;
            }

            var confirmOverwrite = ( Flags & ImGuiFileDialogFlags.ConfirmOverwrite ) == ImGuiFileDialogFlags.ConfirmOverwrite;

            if( IsOk && lastAction && !confirmOverwrite ) {
                return true;
            }

            if( OkResultToConfirm || ( IsOk && lastAction ) && confirmOverwrite ) {
                if( IsOk ) {
                    if( !File.Exists( GetFilePathName() ) ) { // quit dialog
                        return true;
                    }
                    else {
                        IsOk = false;
                        OkResultToConfirm = true;
                    }
                }

                string name = $"The file Already Exists !##{Title}{Id}OverWriteDialog";
                bool res = false;
                bool open = true;

                ImGui.OpenPopup( name );
                if( ImGui.BeginPopupModal( name, ref open, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove ) ) {

                    // TODO: set position at center - ImGui.GetWindowSize() * 0.5f;

                    ImGui.Text( "Would you like to Overwrite it ?" );
                    if( ImGui.Button( "Confirm" ) ) {
                        OkResultToConfirm = false;
                        IsOk = true;
                        res = true;
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.SameLine();
                    if( ImGui.Button( "Cancel" ) ) {
                        OkResultToConfirm = false;
                        IsOk = false;
                        res = false;
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }

                return res;
            }

            return false;
        }

        public static void DisplayVisible( int count, out int preItems, out int showItems, out int postItems, out float itemHeight ) {
            float childHeight = ImGui.GetWindowSize().Y - ImGui.GetCursorPosY();
            var scrollY = ImGui.GetScrollY();
            var style = ImGui.GetStyle();
            itemHeight = ImGui.GetTextLineHeight() + style.ItemSpacing.Y;
            preItems = ( int )Math.Floor( scrollY / itemHeight );
            showItems = ( int )Math.Ceiling( childHeight / itemHeight );
            postItems = count - showItems - preItems;
        }
    }
}
