using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FilePicker.Filter;
using VfxEditor.FilePicker.FolderFiles;
using VfxEditor.FilePicker.SideBar;
using VfxEditor.Utils.Stacks;

namespace VfxEditor.FilePicker {
    public enum SortingField : int {
        None = 0,
        FileName = 1,
        Type = 2,
        Size = 3,
        Date = 4
    }

    [Flags]
    public enum ImGuiFileDialogFlags {
        None = 0,
        ConfirmOverwrite = 1,
        SelectOnly = 2
    }

    public partial class FilePickerDialog {
        public readonly string Id;
        public readonly string Title;
        public readonly bool Modal;
        public readonly bool SelectOnly;
        public readonly bool ConfirmOverwrite;
        public readonly bool FolderDialog;
        public readonly string DefaultExtension;
        public readonly string DefaultFileName;

        private readonly FilePickerSideBar SideBar;
        private readonly FilePickerFilters Filters;

        private string SearchInput = "";
        private string FileNameInput = "";

        private FilePickerFile Selected;
        private readonly List<FilePickerFile> Files = new();
        private readonly List<FilePickerFile> SearchedFiles = new();
        private SortingField CurrentSortingField = SortingField.FileName;
        private readonly bool[] SortDescending = new bool[] { false, false, false, false };

        // ========================

        private bool Visible = false;
        private string CurrentPath;
        private readonly UndoRedoStack<string> PathHistory = new( 25 );

        private readonly List<string> PathParts = new();
        private bool PathEditing = false;
        private string PathInput = "";

        private bool CreatingFolder = false;
        private string NewFolderInput = "";

        public FilePickerDialog(
            string id,
            string title,
            bool modal,
            ImGuiFileDialogFlags flags,
            bool folderDialog,
            string filters,
            string currentPath,
            string defaultFileName,
            string defaultExtension,
            List<FilePickerSidebarItem> recent
        ) {
            Id = id;
            Title = title;
            Modal = modal;
            SelectOnly = flags.HasFlag( ImGuiFileDialogFlags.SelectOnly );
            ConfirmOverwrite = flags.HasFlag( ImGuiFileDialogFlags.ConfirmOverwrite );
            FolderDialog = folderDialog;
            CurrentPath = currentPath;
            DefaultFileName = defaultFileName;
            DefaultExtension = defaultExtension;
            FileNameInput = DefaultFileName;

            SideBar = new( this, recent );
            Filters = new( this, filters );

            SetPath( currentPath );
            Filters.SetSelectedFilter( DefaultExtension );
        }

        public void Show() { Visible = true; }

        public void Hide() { Visible = false; }

        public void SetPath( string path ) {
            CurrentPath = new DirectoryInfo( path ).FullName;
            if( CurrentPath[^1] == Path.DirectorySeparatorChar ) CurrentPath = CurrentPath[0..^1]; // handle selecting a drive, like C: -> C:\
            PathHistory.Add( CurrentPath );

            SearchInput = "";
            Selected = null;
            if( FolderDialog ) FileNameInput = DefaultFileName;
            SideBar.ClearSelected();
            UpdatePathParts();
            UpdateFiles();
        }

        public void UpdateFiles() {
            Files.Clear();
            SearchedFiles.Clear();
            if( !Directory.Exists( CurrentPath ) ) return;
            if( PathParts.Count == 0 ) return;

            if( PathParts.Count > 1 ) {
                Files.Add( new FilePickerFile() {
                    Type = FilePickerFileType.Directory,
                    FilePath = CurrentPath,
                    FileName = ".."
                } );
            }

            var info = new DirectoryInfo( CurrentPath );

            foreach( var dir in info.EnumerateDirectories().OrderBy( d => d.Name ) ) {
                if( string.IsNullOrEmpty( dir.Name ) ) continue;
                Files.Add( FilePickerFile.FromDirectory( dir, CurrentPath ) );
            }

            foreach( var file in info.EnumerateFiles().OrderBy( f => f.Name ) ) {
                if( string.IsNullOrEmpty( file.Name ) ) continue;
                if( Filters.FilterOut( file.Extension ) ) continue;
                Files.Add( FilePickerFile.FromFile( file, CurrentPath ) );
            }

            SortFiles( CurrentSortingField );
        }

        private void UpdatePathParts() {
            PathInput = CurrentPath;
            PathParts.Clear();
            PathParts.AddRange( CurrentPath.Split( Path.DirectorySeparatorChar ) );
        }

        private void SortFiles( SortingField sortingField, bool canChangeOrder = false ) {
            var idx = ( int )sortingField - 1;
            if( canChangeOrder && sortingField == CurrentSortingField ) SortDescending[idx] = !SortDescending[idx]; // flip sorting
            var descending = SortDescending[idx];

            Comparison<FilePickerFile> comparison = sortingField switch {
                SortingField.FileName => descending ? FilePickerFile.SortByFileNameDesc : FilePickerFile.SortByFileNameAsc,
                SortingField.Type => descending ? FilePickerFile.SortByTypeDesc : FilePickerFile.SortByTypeAsc,
                SortingField.Size => descending ? FilePickerFile.SortBySizeDesc : FilePickerFile.SortBySizeAsc,
                SortingField.Date => descending ? FilePickerFile.SortByDateDesc : FilePickerFile.SortByDateAsc,
                _ => null
            };

            if( comparison != null ) Files.Sort( comparison );

            if( sortingField != SortingField.None ) CurrentSortingField = sortingField;

            UpdateSearchedFiles();
        }

        private void UpdateSearchedFiles() {
            SearchedFiles.Clear();

            foreach( var file in Files ) {
                if( !string.IsNullOrEmpty( SearchInput ) && !file.FileName.ToLower().Contains( SearchInput.ToLower() ) ) continue;
                if( FolderDialog && file.Type != FilePickerFileType.Directory ) continue;

                SearchedFiles.Add( file );
            }
        }

        private static string ComposeNewPath( List<string> parts ) {
            if( parts.Count == 1 ) {
                var drivePath = parts[0];
                if( drivePath[^1] != Path.DirectorySeparatorChar ) { // turn C: into C:\
                    drivePath += Path.DirectorySeparatorChar;
                }
                return drivePath;
            }
            return Path.Combine( parts.ToArray() );
        }
    }
}
