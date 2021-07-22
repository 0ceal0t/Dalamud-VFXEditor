using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImGuiFileDialog {
    public partial class FileDialog {
        private static Vector4 PATH_COLOR = new Vector4( 0.188f, 0.188f, 0.2f, 1f );
        private static Vector4 SELECTED_TEXT_COLOR = new Vector4( 0.95f, 0.7f, 0.4f, 1f );
        private static Vector4 DIR_TEXT_COLOR = new Vector4( 0.6f, 0.8f, 0.85f, 1f );
        private static Dictionary<string, char> ICON_MAP;

        public bool Draw() {
            if( !Visible ) return false;

            var res = false;
            var name = Title + "###" + Id;

            bool windowVisible;
            IsOk = false;
            WantsToQuit = false;

            ResetEvents();

            ImGui.SetNextWindowSize( new Vector2( 800, 500 ), ImGuiCond.FirstUseEver );

            if( IsModal && !OkResultToConfirm ) {
                ImGui.OpenPopup( name );
                windowVisible = ImGui.BeginPopupModal( name, ref Visible, ImGuiWindowFlags.NoScrollbar );
            }
            else {
                windowVisible = ImGui.Begin( name, ref Visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoNav );
            }

            if( windowVisible ) {
                if( !Visible ) { // window closed
                    IsOk = false;
                    return true;
                }

                if( SelectedFilter.Empty() && ( Filters.Count > 0 ) ) {
                    SelectedFilter = Filters[0];
                }

                if( Files.Count == 0 ) {
                    if( !string.IsNullOrEmpty( DefaultFileName ) ) {
                        SetDefaultFileName( DefaultFileName );
                        SetSelectedFilterWithExt( DefaultExtension );
                    }
                    else if( IsDirectoryMode() ) {
                        SetDefaultFileName( DefaultFileName );
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

            DrawPathComposer();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            DrawSearchBar();
        }

        private void DrawPathComposer() {
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{ ( PathInputActivated ? ( char )FontAwesomeIcon.Times : ( char )FontAwesomeIcon.Edit )}" ) ) {
                PathInputActivated = !PathInputActivated;
            }
            ImGui.PopFont();

            ImGui.SameLine();

            if( PathDecomposition.Count > 0 ) {
                ImGui.SameLine();

                if( PathInputActivated ) {
                    ImGui.PushItemWidth( ImGui.GetContentRegionAvail().X );
                    ImGui.InputText( "##pathedit", ref PathInputBuffer, 255 );
                    ImGui.PopItemWidth();
                }
                else {
                    for( int idx = 0; idx < PathDecomposition.Count; idx++ ) {
                        if( idx > 0 ) {
                            ImGui.SameLine();
                            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 3 );
                        }

                        ImGui.PushID( idx );
                        ImGui.PushStyleColor( ImGuiCol.Button, PATH_COLOR );
                        var click = ImGui.Button( PathDecomposition[idx] );
                        ImGui.PopStyleColor();
                        ImGui.PopID();

                        if( click ) {
                            CurrentPath = ComposeNewPath( PathDecomposition.GetRange( 0, idx + 1 ) );
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
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Home}" ) ) {
                SetPath( "." );
            }
            ImGui.PopFont();

            if( ImGui.IsItemHovered() ) {
                ImGui.SetTooltip( "Reset to current directory" );
            }

            ImGui.SameLine();

            DrawDirectoryCreation();

            if( !CreateDirectoryMode ) {
                ImGui.SameLine();
                ImGui.Text( "Search :" );
                ImGui.SameLine();
                ImGui.PushItemWidth( ImGui.GetContentRegionAvail().X );
                var edited = ImGui.InputText( "##InputImGuiFileDialogSearchField", ref SearchBuffer, 255 );
                ImGui.PopItemWidth();
                if( edited ) {
                    ApplyFilteringOnFileList();
                }
            }
        }

        private void DrawDirectoryCreation() {
            if( Flags.HasFlag( ImGuiFileDialogFlags.DisableCreateDirectoryButton ) ) return;

            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.FolderPlus}" ) ) {
                if( !CreateDirectoryMode ) {
                    CreateDirectoryMode = true;
                    CreateDirectoryBuffer = "";
                }
            }
            ImGui.PopFont();

            if( ImGui.IsItemHovered() ) {
                ImGui.SetTooltip( "Create Directory" );
            }

            if( CreateDirectoryMode ) {
                ImGui.SameLine();
                ImGui.Text( "New Directory Name" );

                ImGui.SameLine();
                ImGui.PushItemWidth( ImGui.GetContentRegionAvail().X - 100f );
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
        }

        private void DrawContent() {
            var size = ImGui.GetContentRegionAvail() - new Vector2( 0, FooterHeight );

            if(!Flags.HasFlag(ImGuiFileDialogFlags.HideSideBar)) {
                ImGui.BeginChild( "##FileDialog_ColumnChild", size);
                ImGui.Columns( 2, "##FileDialog_Columns" );

                DrawSideBar( new Vector2( 150, size.Y ) );

                ImGui.SetColumnWidth( 0, 150 );
                ImGui.NextColumn();

                DrawFileListView( size - new Vector2(160, 0) );

                ImGui.Columns( 1 );
                ImGui.EndChild();
            }
            else {
                DrawFileListView( size );
            }
        }

        private void DrawSideBar(Vector2 size) {
            ImGui.BeginChild( "##FileDialog_SideBar", size );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            foreach(var drive in Drives) {
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Selectable( $"{drive.Icon}##{drive.Text}", drive.Text == SelectedSideBar ) ) {
                    SetPath( drive.Location );
                    SelectedSideBar = drive.Text;
                }
                ImGui.PopFont();

                ImGui.SameLine(25);

                ImGui.Text( drive.Text );
            }

            foreach(var quick in QuickAccess) {
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Selectable( $"{quick.Icon}##{quick.Text}", quick.Text == SelectedSideBar ) ) {
                    SetPath( quick.Location );
                    SelectedSideBar = quick.Text;
                }
                ImGui.PopFont();

                ImGui.SameLine(25);

                ImGui.Text( quick.Text );
            }

            ImGui.EndChild();
        }

        private unsafe void DrawFileListView( Vector2 size ) {
            ImGui.BeginChild( "##FileDialog_FileList", size );

            ImGuiTableFlags tableFlags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg | ImGuiTableFlags.Hideable | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoHostExtendX;
            if( ImGui.BeginTable( "##FileTable", 4, tableFlags, size ) ) {
                ImGui.TableSetupScrollFreeze( 0, 1 );

                var hideType = Flags.HasFlag( ImGuiFileDialogFlags.HideColumnType );
                var hideSize = Flags.HasFlag( ImGuiFileDialogFlags.HideColumnSize );
                var hideDate = Flags.HasFlag( ImGuiFileDialogFlags.HideColumnDate );

                ImGui.TableSetupColumn( " File Name", ImGuiTableColumnFlags.WidthStretch, -1, 0 );
                ImGui.TableSetupColumn( "Type", ImGuiTableColumnFlags.WidthFixed | ( hideType ? ImGuiTableColumnFlags.DefaultHide : ImGuiTableColumnFlags.None ), -1, 1 );
                ImGui.TableSetupColumn( "Size", ImGuiTableColumnFlags.WidthFixed | ( hideSize ? ImGuiTableColumnFlags.DefaultHide : ImGuiTableColumnFlags.None ), -1, 2 );
                ImGui.TableSetupColumn( "Date", ImGuiTableColumnFlags.WidthFixed | ( hideDate ? ImGuiTableColumnFlags.DefaultHide : ImGuiTableColumnFlags.None ), -1, 3 );

                ImGui.TableNextRow( ImGuiTableRowFlags.Headers );
                for( int column = 0; column < 4; column++ ) {
                    ImGui.TableSetColumnIndex( column );
                    var columnName = ImGui.TableGetColumnName( column );
                    ImGui.PushID( column );
                    ImGui.TableHeader( columnName );
                    ImGui.PopID();
                    if( ImGui.IsItemClicked() ) {
                        if( column == 0 ) {
                            SortFields( SortingField.FileName, true );
                        }
                        else if( column == 1 ) {
                            SortFields( SortingField.Type, true );
                        }
                        else if( column == 2 ) {
                            SortFields( SortingField.Size, true );
                        }
                        else {
                            SortFields( SortingField.Date, true );
                        }
                    }
                }

                if( FilteredFiles.Count > 0 ) {
                    ImGuiListClipperPtr clipper;
                    unsafe {
                        clipper = new ImGuiListClipperPtr( ImGuiNative.ImGuiListClipper_ImGuiListClipper() );
                    }

                    lock( FilesLock ) {
                        clipper.Begin( FilteredFiles.Count );
                        while( clipper.Step() ) {
                            for( var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++ ) {
                                if( i < 0 ) continue;

                                var file = FilteredFiles[i];
                                var selected = SelectedFileNames.Contains( file.FileName );
                                var needToBreak = false;

                                if( file.Type == FileStructType.Directory ) ImGui.PushStyleColor( ImGuiCol.Text, DIR_TEXT_COLOR );
                                if( selected ) ImGui.PushStyleColor( ImGuiCol.Text, SELECTED_TEXT_COLOR );

                                ImGui.TableNextRow();

                                if( ImGui.TableNextColumn() ) {
                                    needToBreak = SelectableItem( file, selected );
                                }
                                if( ImGui.TableNextColumn() ) {
                                    ImGui.Text( file.Ext );
                                }
                                if( ImGui.TableNextColumn() ) {
                                    if( file.Type == FileStructType.File ) {
                                        ImGui.Text( file.FormattedFileSize + " " );
                                    }
                                    else {
                                        ImGui.Text( " " );
                                    }
                                }
                                if( ImGui.TableNextColumn() ) {
                                    var sz = ImGui.CalcTextSize( file.FileModifiedDate );
                                    ImGui.PushItemWidth( sz.X + 5 );
                                    ImGui.Text( file.FileModifiedDate + " " );
                                    ImGui.PopItemWidth();
                                }

                                if( selected ) ImGui.PopStyleColor();
                                if( file.Type == FileStructType.Directory ) ImGui.PopStyleColor();

                                if( needToBreak ) break;
                            }
                        }

                        clipper.End();
                    }
                }

                if( PathInputActivated ) {
                    if( ImGui.IsKeyReleased( ImGui.GetKeyIndex( ImGuiKey.Enter ) ) ) {
                        if(Directory.Exists(PathInputBuffer)) SetPath( PathInputBuffer );
                        PathInputActivated = false;
                    }
                    if( ImGui.IsKeyReleased( ImGui.GetKeyIndex( ImGuiKey.Escape ) ) ) {
                        PathInputActivated = false;
                    }
                }

                ImGui.EndTable();
            }

            if( PathClicked ) {
                SetPath( CurrentPath );
            }

            ImGui.EndChild();
        }

        private bool SelectableItem( FileStruct file, bool selected ) {
            var flags = ImGuiSelectableFlags.AllowDoubleClick | ImGuiSelectableFlags.SpanAllColumns;

            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.Text( file.Type == FileStructType.Directory ? $"{( char )FontAwesomeIcon.Folder}" : $"{GetIcon(file.Ext)}" );
            ImGui.PopFont();

            ImGui.SameLine(25f);

            // TODO: key exploration
            if( ImGui.Selectable( file.FileName, selected, flags ) ) {
                if( file.Type == FileStructType.Directory ) {
                    if( ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left ) ) {
                        PathClicked = SelectDirectory( file );
                        return true;
                    }
                    else if( IsDirectoryMode() ) {
                        SelectFileName( file );
                    }
                }
                else {
                    SelectFileName( file );
                    if( ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left ) ) {
                        WantsToQuit = true;
                        IsOk = true;
                    }
                }
            }

            return false;
        }

        private char GetIcon( string ext ) {
            if( ICON_MAP == null ) {
                ICON_MAP = new();
                AddToIconMap( new[] { "mp4", "gif", "mov", "avi" }, ( char )FontAwesomeIcon.FileVideo );
                AddToIconMap( new[] { "pdf" }, ( char )FontAwesomeIcon.FilePdf );
                AddToIconMap( new[] { "png", "jpg", "jpeg", "tiff" }, ( char )FontAwesomeIcon.FileImage );
                AddToIconMap( new[] { "cs", "json", "cpp", "h", "py", "xml", "yaml", "js", "html", "css", "ts", "java" }, ( char )FontAwesomeIcon.FileCode );
                AddToIconMap( new[] { "txt", "md" }, ( char )FontAwesomeIcon.FileAlt );
                AddToIconMap( new[] { "zip", "7z", "gz", "tar" }, ( char )FontAwesomeIcon.FileArchive );
                AddToIconMap( new[] { "mp3", "m4a", "ogg", "wav" }, ( char )FontAwesomeIcon.FileAudio );
                AddToIconMap( new[] { "csv" }, ( char )FontAwesomeIcon.FileCsv );
            }

            return ICON_MAP.TryGetValue(ext.ToLower(), out var icon) ? icon : ( char )FontAwesomeIcon.File;
        }

        private void AddToIconMap( string[] extensions, char icon ) {
            foreach(var ext in extensions) {
                ICON_MAP[ext] = icon;
            }
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
                string newPath = Path.Combine( CurrentPath, file.FileName );

                if( Directory.Exists( newPath ) ) {
                    CurrentPath = newPath;
                }
                pathClick = true;
            }

            return pathClick;
        }

        private void SelectFileName( FileStruct file ) {
            if( ImGui.GetIO().KeyCtrl ) {
                if(SelectionCountMax == 0) { // infinite select
                    if( !SelectedFileNames.Contains( file.FileName ) ) {
                        AddFileNameInSelection( file.FileName, true );
                    }
                    else {
                        RemoveFileNameInSelection( file.FileName );
                    }
                }
                else {
                    if(SelectedFileNames.Count < SelectionCountMax) {
                        if( !SelectedFileNames.Contains( file.FileName ) ) {
                            AddFileNameInSelection( file.FileName, true );
                        }
                        else {
                            RemoveFileNameInSelection( file.FileName );
                        }
                    }
                }
            }
            else if( ImGui.GetIO().KeyShift ) {
                if(SelectionCountMax != 1) { // can select a block
                    SelectedFileNames.Clear();

                    var startMultiSelection = false;
                    var fileNameToSelect = file.FileName;
                    string savedLastSelectedFileName = "";

                    foreach(var f in FilteredFiles) {
                        // select top-to-bottom
                        if( f.FileName == LastSelectedFileName) { // start (the previously selected one)
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

                        // select bottom-to-top
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

            var width = ImGui.GetContentRegionAvail().X - 100;
            if( Filters.Count > 0 ) {
                width -= 150f;
            }

            var selectOnly = Flags.HasFlag( ImGuiFileDialogFlags.SelectOnly );

            ImGui.PushItemWidth( width );
            if( selectOnly ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
            ImGui.InputText( "##FileName", ref FileNameBuffer, 255, selectOnly ? ImGuiInputTextFlags.ReadOnly : ImGuiInputTextFlags.None );
            if( selectOnly ) ImGui.PopStyleVar();
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

            ImGui.SameLine();

            var disableOk = string.IsNullOrEmpty( FileNameBuffer ) || ( selectOnly && !IsItemSelected() );
            if( disableOk ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );

            if( ImGui.Button( "Ok" ) && !disableOk ) {
                IsOk = true;
                res = true;
            }

            if( disableOk ) ImGui.PopStyleVar();

            ImGui.SameLine();

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

        private bool IsItemSelected() {
            if( SelectedFileNames.Count > 0 ) return true;
            if( IsDirectoryMode()) return true; // current directory
            return false;
        }

        private bool ConfirmOrOpenOverWriteFileDialogIfNeeded( bool lastAction ) {
            if( IsDirectoryMode() ) return lastAction;
            if( !IsOk && lastAction ) return true; // no need to confirm anything, since it was cancelled

            var confirmOverwrite = Flags.HasFlag(ImGuiFileDialogFlags.ConfirmOverwrite);

            if( IsOk && lastAction && !confirmOverwrite ) return true;

            if( OkResultToConfirm || ( IsOk && lastAction ) && confirmOverwrite ) { // if waiting on a confirmation, or need to start one
                if( IsOk ) {
                    if( !File.Exists( GetFilePathName() ) ) { // quit dialog, it doesn't exist anyway
                        return true;
                    }
                    else { // already exists, open dialog to confirm overwrite
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
    }
}
