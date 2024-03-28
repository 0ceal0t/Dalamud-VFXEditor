using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VfxEditor.FileBrowser.Filter;
using VfxEditor.FileBrowser.FolderFiles;
using VfxEditor.FileBrowser.Preview;
using VfxEditor.FileBrowser.SideBar;
using VfxEditor.Utils.Stacks;

namespace VfxEditor.FileBrowser {
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

    public partial class FileBrowserDialog {
        public readonly string Id;
        public readonly string Title;
        public readonly bool Modal;
        public readonly bool SelectOnly;
        public readonly bool ConfirmOverwrite;
        public readonly bool FolderDialog;
        public readonly bool EnablePreview;
        public readonly string DefaultExtension;
        public readonly string DefaultFileName;
        public readonly CommandManager Command;

        private readonly FileBrowserSideBar SideBar;
        private readonly FileBrowserFilters Filters;
        private readonly FileBrowserPreview Preview;

        private string SearchInput = "";
        private string FileNameInput = "";

        private FileBrowserFile Selected;
        private readonly List<FileBrowserFile> Files = [];
        private readonly List<FileBrowserFile> SearchedFiles = [];
        private SortingField CurrentSortingField = SortingField.FileName;
        private readonly bool[] SortDescending = [false, false, false, false];

        // ========================

        private bool Visible = false;
        private string CurrentPath;
        private readonly UndoRedoStack<(string, string)> PathHistory = new( 25 );

        private readonly List<string> PathParts = [];
        private bool JustDrive => PathParts.Count == 1;
        private bool PathEditing = false;
        private string PathInput = "";

        private bool CreatingFolder = false;
        private string NewFolderInput = "";

        public FileBrowserDialog(
            string id,
            string title,
            bool modal,
            ImGuiFileDialogFlags flags,
            bool folderDialog,
            string filters,
            string currentPath,
            string defaultFileName,
            string defaultExtension,
            CommandManager command
        ) {
            Id = id;
            Title = title;
            Modal = modal;
            SelectOnly = flags.HasFlag( ImGuiFileDialogFlags.SelectOnly );
            ConfirmOverwrite = flags.HasFlag( ImGuiFileDialogFlags.ConfirmOverwrite ) && !Plugin.Configuration.FileBrowserOverwriteDontAsk;
            FolderDialog = folderDialog;
            CurrentPath = currentPath;
            DefaultFileName = defaultFileName;
            DefaultExtension = defaultExtension;
            FileNameInput = DefaultFileName;
            Command = command;

            SideBar = new( this );
            Filters = new( this, filters );
            Preview = new();

            // Decide whether preview is even necessary
            EnablePreview = Filters.Filters.Any( x => x.HasExtension( FileBrowserPreview.ImageExtensions ) );

            SetPath( currentPath, false );
            Filters.SetSelectedFilter( DefaultExtension );
        }

        public void Show() { Visible = true; }

        public void Hide() { Visible = false; }

        public void SetPath( string path, bool record ) {
            var prevPath = CurrentPath;
            var partCount = path.Split( Path.DirectorySeparatorChar ).Length;
            CurrentPath = new DirectoryInfo( partCount == 1 ? $"{path}{Path.DirectorySeparatorChar}" : path ).FullName;

            if( CurrentPath[^1] == Path.DirectorySeparatorChar ) CurrentPath = CurrentPath[0..^1]; // handle selecting a drive, like C: -> C:\
            if( record ) PathHistory.Add( (prevPath, CurrentPath) );

            SearchInput = "";
            Selected = null;
            if( FolderDialog ) FileNameInput = DefaultFileName;
            SideBar.Clear();
            Preview.Clear();
            UpdatePathParts();
            UpdateFiles();
        }

        public async void UpdateFiles() {
            await Task.Run( () => {
                Files.Clear();
                SearchedFiles.Clear();
                if( !Directory.Exists( CurrentPath ) ) return;
                if( PathParts.Count == 0 ) return;

                if( PathParts.Count > 1 ) {
                    Files.Add( new FileBrowserFile() {
                        Type = FilePickerFileType.Directory,
                        FilePath = CurrentPath,
                        FileName = ".."
                    } );
                }

                var info = new DirectoryInfo( JustDrive ? $"{CurrentPath}{Path.DirectorySeparatorChar}" : CurrentPath );

                foreach( var dir in info.EnumerateDirectories().OrderBy( d => d.Name ) ) {
                    if( string.IsNullOrEmpty( dir.Name ) ) continue;
                    Files.Add( FileBrowserFile.FromDirectory( dir, CurrentPath ) );
                }

                foreach( var file in info.EnumerateFiles().OrderBy( f => f.Name ) ) {
                    if( string.IsNullOrEmpty( file.Name ) ) continue;
                    if( Filters.FilterOut( file.Extension ) ) continue;
                    Files.Add( FileBrowserFile.FromFile( file, CurrentPath ) );
                }

                SortFiles( CurrentSortingField );
            } );
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

            Comparison<FileBrowserFile> comparison = sortingField switch {
                SortingField.FileName => descending ? FileBrowserFile.SortByFileNameDesc : FileBrowserFile.SortByFileNameAsc,
                SortingField.Type => descending ? FileBrowserFile.SortByTypeDesc : FileBrowserFile.SortByTypeAsc,
                SortingField.Size => descending ? FileBrowserFile.SortBySizeDesc : FileBrowserFile.SortBySizeAsc,
                SortingField.Date => descending ? FileBrowserFile.SortByDateDesc : FileBrowserFile.SortByDateAsc,
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
            return Path.Combine( [.. parts] );
        }

        public void Dispose() => Preview.Dispose();
    }
}
