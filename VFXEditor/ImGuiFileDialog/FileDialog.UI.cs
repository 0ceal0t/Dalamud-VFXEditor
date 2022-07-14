using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using TeximpNet.DDS;
using VFXEditor.Texture;

namespace ImGuiFileDialog {
    public partial class FileDialog {
        private static Vector4 PATH_DECOMP_COLOR = new( 0.188f, 0.188f, 0.2f, 1f );
        private static Vector4 SELECTED_TEXT_COLOR = new( 1.00000000000f, 0.33333333333f, 0.33333333333f, 1f );
        private static Vector4 DIR_TEXT_COLOR = new( 0.54509803922f, 0.91372549020f, 0.99215686275f, 1f );
        private static Vector4 CODE_TEXT_COLOR = new( 0.94509803922f, 0.98039215686f, 0.54901960784f, 1f );
        private static Vector4 MISC_TEXT_COLOR = new( 1.00000000000f, 0.47450980392f, 0.77647058824f, 1f );
        private static Vector4 IMAGE_TEXT_COLOR = new( 0.31372549020f, 0.98039215686f, 0.48235294118f, 1f );
        private static Vector4 STANDARD_TEXT_COLOR = new( 1f );

        private static readonly List<string> ImageExtensions = new() { "jpeg", "jpg", "png", "dds", "atex" };

        private struct IconColorItem {
            public char Icon;
            public Vector4 Color;
        }

        private static Dictionary<string, IconColorItem> ICON_MAP;

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
                windowVisible = ImGui.Begin( name, ref Visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoDocking );
            }

            if( windowVisible ) {
                if( !Visible ) { // window closed
                    IsOk = false;

                    if( IsModal && !OkResultToConfirm ) {
                        ImGui.EndPopup();
                    }
                    else {
                        ImGui.End();
                    }

                    return true;
                }

                if( SelectedFilter.Empty() && ( Filters.Count > 0 ) ) {
                    SelectedFilter = Filters[0];
                }

                if( Files.Count == 0 ) {
                    if( !string.IsNullOrEmpty( DefaultFileName ) ) {
                        SetDefaultFileName();
                        SetSelectedFilterWithExt( DefaultExtension );
                    }
                    else if( IsDirectoryMode() ) {
                        SetDefaultFileName();
                    }

                    ScanDir( CurrentPath );
                }

                DrawHeader();
                DrawContent();
                res = DrawFooter();

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
                    for( var idx = 0; idx < PathDecomposition.Count; idx++ ) {
                        if( idx > 0 ) {
                            ImGui.SameLine();
                            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 3 );
                        }

                        ImGui.PushID( idx );
                        ImGui.PushStyleColor( ImGuiCol.Button, PATH_DECOMP_COLOR );
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

            if( !Flags.HasFlag( ImGuiFileDialogFlags.HideSideBar ) ) {
                ImGui.BeginChild( "##FileDialog_ColumnChild", size );
                ImGui.Columns( 2, "##FileDialog_Columns" );

                DrawSideBar( new Vector2( 150, size.Y ) );

                ImGui.SetColumnWidth( 0, 150 );
                ImGui.NextColumn();

                DrawFileListView( size - new Vector2( 160, 0 ) );

                ImGui.Columns( 1 );
                ImGui.EndChild();
            }
            else {
                DrawFileListView( size );
            }
        }

        private void DrawSideBar( Vector2 size ) {
            ImGui.BeginChild( "##FileDialog_SideBar", ( PreviewWrap != null ) ? size - new Vector2( 0, size.X + 5 ) : size );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            foreach( var drive in Drives ) {
                DrawSideBarItem( drive );
            }

            foreach( var quick in QuickAccess ) {
                DrawSideBarItem( quick );
            }

            if( Recent != null ) {
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 10 );
                foreach( var recent in Recent ) {
                    DrawSideBarItem( recent );
                }
            }

            ImGui.EndChild();

            if( PreviewWrap != null ) {
                var width = size.X;
                var previewSize = new Vector2( PreviewWrap.Width, PreviewWrap.Height );
                var ratio = Math.Min( width / previewSize.X, width / previewSize.Y );
                var newSize = previewSize *= ratio;
                var padding = ( new Vector2( width ) - newSize ) / 2;
                ImGui.SetCursorPos( ImGui.GetCursorPos() + padding );
                ImGui.Image( PreviewWrap.ImGuiHandle, newSize );
            }
        }

        private void DrawSideBarItem( SideBarItem item ) {
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Selectable( $"{item.Icon}##{item.Text}", item.Text == SelectedSideBar ) ) {
                PreviewWrap?.Dispose();
                PreviewWrap = null;

                SetPath( item.Location );
                SelectedSideBar = item.Text;
            }
            ImGui.PopFont();

            ImGui.SameLine( 25 );
            ImGui.Text( item.Text );
        }

        private unsafe void DrawFileListView( Vector2 size ) {
            ImGui.BeginChild( "##FileDialog_FileList", size );

            var tableFlags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg | ImGuiTableFlags.Hideable | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoHostExtendX;
            if( ImGui.BeginTable( "##FileTable", 4, tableFlags, size ) ) {
                ImGui.TableSetupScrollFreeze( 0, 1 );

                ImGui.TableSetupColumn( " File Name", ImGuiTableColumnFlags.WidthStretch, -1, 0 );
                ImGui.TableSetupColumn( "Type", ImGuiTableColumnFlags.WidthFixed, -1, 1 );
                ImGui.TableSetupColumn( "Size", ImGuiTableColumnFlags.WidthFixed, -1, 2 );
                ImGui.TableSetupColumn( "Date", ImGuiTableColumnFlags.WidthFixed, -1, 3 );

                ImGui.TableNextRow( ImGuiTableRowFlags.Headers );
                for( var column = 0; column < 4; column++ ) {
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

                                var dir = file.Type == FileStructType.Directory;
                                var item = !dir ? GetIcon( file.Ext ) : new IconColorItem {
                                    Color = DIR_TEXT_COLOR,
                                    Icon = ( char )FontAwesomeIcon.Folder
                                };

                                ImGui.PushStyleColor( ImGuiCol.Text, item.Color );
                                if( selected ) ImGui.PushStyleColor( ImGuiCol.Text, SELECTED_TEXT_COLOR );

                                ImGui.TableNextRow();

                                if( ImGui.TableNextColumn() ) {
                                    needToBreak = SelectableItem( file, selected, item.Icon );
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
                                ImGui.PopStyleColor();

                                if( needToBreak ) break;
                            }
                        }

                        clipper.End();
                    }
                }

                if( PathInputActivated ) {
                    if( ImGui.IsKeyReleased( ImGuiKey.Enter ) ) {
                        if( Directory.Exists( PathInputBuffer ) ) SetPath( PathInputBuffer );
                        PathInputActivated = false;
                    }
                    if( ImGui.IsKeyReleased( ImGuiKey.Escape ) ) {
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

        private bool SelectableItem( FileStruct file, bool selected, char icon ) {
            var flags = ImGuiSelectableFlags.AllowDoubleClick | ImGuiSelectableFlags.SpanAllColumns;

            ImGui.PushFont( UiBuilder.IconFont );

            ImGui.Text( $"{icon}" );
            ImGui.PopFont();

            ImGui.SameLine( 25f );

            if( ImGui.Selectable( file.FileName, selected, flags ) ) {
                PreviewWrap?.Dispose();
                PreviewWrap = null;

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

                    LoadTexturePreview( file );
                }
            }

            return false;
        }

        private void LoadTexturePreview( FileStruct file ) {
            if( !DoLoadPreview ) return;
            if( ImageExtensions.Contains( file.Ext.ToLower() ) ) {
                Task.Run( async () => {

                    lock( PreviewLock ) {
                        var path = Path.Combine( file.FilePath, file.FileName );
                        if( file.Ext.ToLower() == "dds" ) {
                            var ddsFile = DDSFile.Read( path );

                            using( var ms = new MemoryStream() ) {
                                ddsFile.Write( ms );
                                using var br = new BinaryReader( ms );

                                br.BaseStream.Seek( 12, SeekOrigin.Begin );
                                var height = br.ReadInt32();
                                var width = br.ReadInt32();

                                br.BaseStream.Seek( 128, SeekOrigin.Begin ); // skip header
                                var uncompressedLength = ms.Length - 128;
                                var data = new byte[uncompressedLength];
                                br.Read( data, 0, ( int )uncompressedLength );

                                var format = VFXTexture.DXGItoTextureFormat( ddsFile.Format );
                                var convertedData = VFXTexture.BGRA_to_RGBA( VFXTexture.Convert( data, format, width, height ) );
                                var temp = PluginInterface.UiBuilder.LoadImageRaw( convertedData, width, height, 4 );
                                PreviewWrap = temp;
                            }

                            ddsFile.Dispose();
                        }
                        else if( file.Ext.ToLower() == "atex" ) {
                            var tex = VFXTexture.LoadFromLocal( path );
                            var temp = PluginInterface.UiBuilder.LoadImageRaw( tex.ImageData, tex.Header.Width, tex.Header.Height, 4 );
                            PreviewWrap = temp;
                        }
                        else {
                            var temp = PluginInterface.UiBuilder.LoadImage( path );
                            PreviewWrap = temp;
                        }
                    }

                } );
            }
        }

        private static IconColorItem GetIcon( string ext ) {
            if( ICON_MAP == null ) {
                ICON_MAP = new();
                AddToIconMap( new[] { "mp4", "gif", "mov", "avi" }, ( char )FontAwesomeIcon.FileVideo, MISC_TEXT_COLOR );
                AddToIconMap( new[] { "pdf" }, ( char )FontAwesomeIcon.FilePdf, MISC_TEXT_COLOR );
                AddToIconMap( new[] { "png", "jpg", "jpeg", "tiff", "dds", "atex" }, ( char )FontAwesomeIcon.FileImage, IMAGE_TEXT_COLOR );
                AddToIconMap( new[] { "cs", "json", "cpp", "h", "py", "xml", "yaml", "js", "html", "css", "ts", "java" }, ( char )FontAwesomeIcon.FileCode, CODE_TEXT_COLOR );
                AddToIconMap( new[] { "txt", "md" }, ( char )FontAwesomeIcon.FileAlt, STANDARD_TEXT_COLOR );
                AddToIconMap( new[] { "zip", "7z", "gz", "tar" }, ( char )FontAwesomeIcon.FileArchive, MISC_TEXT_COLOR );
                AddToIconMap( new[] { "mp3", "m4a", "ogg", "wav" }, ( char )FontAwesomeIcon.FileAudio, MISC_TEXT_COLOR );
                AddToIconMap( new[] { "csv" }, ( char )FontAwesomeIcon.FileCsv, MISC_TEXT_COLOR );
            }

            return ICON_MAP.TryGetValue( ext.ToLower(), out var icon ) ? icon : new IconColorItem {
                Icon = ( char )FontAwesomeIcon.File,
                Color = STANDARD_TEXT_COLOR
            };
        }

        private static void AddToIconMap( string[] extensions, char icon, Vector4 color ) {
            foreach( var ext in extensions ) {
                ICON_MAP[ext] = new IconColorItem {
                    Icon = icon,
                    Color = color
                };
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
                var newPath = Path.Combine( CurrentPath, file.FileName );

                if( Directory.Exists( newPath ) ) {
                    CurrentPath = newPath;
                }
                pathClick = true;
            }

            return pathClick;
        }

        private void SelectFileName( FileStruct file ) {
            if( ImGui.GetIO().KeyCtrl ) {
                if( SelectionCountMax == 0 ) { // infinite select
                    if( !SelectedFileNames.Contains( file.FileName ) ) {
                        AddFileNameInSelection( file.FileName, true );
                    }
                    else {
                        RemoveFileNameInSelection( file.FileName );
                    }
                }
                else {
                    if( SelectedFileNames.Count < SelectionCountMax ) {
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
                if( SelectionCountMax != 1 ) { // can select a block
                    SelectedFileNames.Clear();

                    var startMultiSelection = false;
                    var fileNameToSelect = file.FileName;
                    var savedLastSelectedFileName = "";

                    foreach( var f in FilteredFiles ) {
                        // select top-to-bottom
                        if( f.FileName == LastSelectedFileName ) { // start (the previously selected one)
                            startMultiSelection = true;
                            AddFileNameInSelection( LastSelectedFileName, false );
                        }
                        else if( startMultiSelection ) {
                            if( SelectionCountMax == 0 ) {
                                AddFileNameInSelection( f.FileName, false );
                            }
                            else {
                                if( SelectedFileNames.Count < SelectionCountMax ) {
                                    AddFileNameInSelection( f.FileName, false );
                                }
                                else {
                                    startMultiSelection = false;
                                    if( !string.IsNullOrEmpty( savedLastSelectedFileName ) ) {
                                        LastSelectedFileName = savedLastSelectedFileName;
                                    }
                                    break;
                                }
                            }
                        }

                        // select bottom-to-top
                        if( f.FileName == fileNameToSelect ) {
                            if( !startMultiSelection ) {
                                savedLastSelectedFileName = LastSelectedFileName;
                                LastSelectedFileName = fileNameToSelect;
                                fileNameToSelect = savedLastSelectedFileName;
                                startMultiSelection = true;
                                AddFileNameInSelection( LastSelectedFileName, false );
                            }
                            else {
                                startMultiSelection = false;
                                if( !string.IsNullOrEmpty( savedLastSelectedFileName ) ) {
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

        private void AddFileNameInSelection( string name, bool setLastSelection ) {
            SelectedFileNames.Add( name );
            if( SelectedFileNames.Count == 1 ) {
                FileNameBuffer = name;
            }
            else {
                FileNameBuffer = $"{SelectedFileNames.Count} files Selected";
            }

            if( setLastSelection ) {
                LastSelectedFileName = name;
            }
        }

        private void RemoveFileNameInSelection( string name ) {
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
                    var idx = 0;
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
            if( IsDirectoryMode() ) return true; // current directory
            return false;
        }

        private bool ConfirmOrOpenOverWriteFileDialogIfNeeded( bool lastAction ) {
            if( IsDirectoryMode() ) return lastAction;
            if( !IsOk && lastAction ) return true; // no need to confirm anything, since it was cancelled

            var confirmOverwrite = Flags.HasFlag( ImGuiFileDialogFlags.ConfirmOverwrite );

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

                var name = $"The file Already Exists !##{Title}{Id}OverWriteDialog";
                var res = false;
                var open = true;

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
