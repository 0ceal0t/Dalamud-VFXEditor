using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ImGuiNET;

namespace ImGuiFileDialog {
    [Flags]
    public enum ImGuiFileDialogFlags {
        None = 0,
        ConfirmOverwrite = 1,
        CheckIfExists = 2,
        DontShowHiddenFiles = 3,
        DisableCreateDirectoryButton = 4,
        HideColumnType = 5,
        HideColumnSize = 6,
        HideColumnDate = 7,
    }

    public partial class FileDialog {
        // TODO: save path
        // TODO: save filter

        private string Title;
        private int SelectionCountMax;
        private ImGuiFileDialogFlags Flags;
        private string Id;

        private bool Visible;

        private string CurrentPath;

        private string DefaultExtension;
        private string DefaultFileName;
        private string FileNameBuffer = "";

        private List<string> PathDecomposition = new();
        private bool PathClicked = true;
        private bool PathInputActivated = false;
        private string PathInputBuffer = "";

        private bool IsModal = false;
        private bool OkResultToConfirm = false;

        private bool IsOk;
        private bool WantsToQuit;

        private bool ShowDrives = false;
        private bool DrivesClicked = false;

        private bool AnyWindowsHovered; // TODO

        private bool CreateDirectoryMode = false;
        private string CreateDirectoryBuffer = "";

        private string SearchBuffer = "";

        private string LastSelectedFileName = "";
        private List<string> SelectedFileNames = new();

        private float FooterHeight = 0;

        public FileDialog(string id, string title, string filters, string path, string defaultFileName, string defaultExtension, int selectionCountMax, bool isModal, ImGuiFileDialogFlags flags) {
            Id = id;
            Title = title;
            Flags = flags;
            SelectionCountMax = selectionCountMax;
            IsModal = isModal;

            CurrentPath = path;
            DefaultExtension = defaultExtension;

            ParseFilters( filters );
            SetSelectedFilterWithExt( DefaultExtension );
            SetDefaultFileName( defaultFileName );
            SetPath( CurrentPath );
        }

        public void Show() {
            Visible = true;
        }

        public void Hide() {
            Visible = false;
        }

        public bool GetIsOk() {
            return IsOk;
        }

        public bool GetWantsToQuit() {
            return WantsToQuit;
        }

        private string GetFilePathName() {
            var result = GetCurrentPath();
            var fileName = GetCurrentFileName();

            if(!string.IsNullOrEmpty(fileName)) {
                result = Path.Combine( result, fileName );
            }

            return result;
        }

        public string GetCurrentPath() {
            if(IsDirectoryMode()) { // combine path file with directory input
                var selectedDirectory = FileNameBuffer;
                if(!string.IsNullOrEmpty(selectedDirectory) && selectedDirectory != ".") {
                    return string.IsNullOrEmpty( CurrentPath ) ? selectedDirectory : Path.Combine( CurrentPath, selectedDirectory );
                }
            }

            return CurrentPath;
        }

        public string GetCurrentFileName() {
            if( !IsDirectoryMode() ) { // add extension
                var result = FileNameBuffer;

                // a collection like {.cpp, .h}, so can't decide on an extension
                if( SelectedFilter.CollectionFilters != null && SelectedFilter.CollectionFilters.Count > 0 ) {
                    return result;
                }

                // a single one, like .cpp
                if( !SelectedFilter.Filter.Contains( '*' ) && result != SelectedFilter.Filter ) {
                    var lastPoint = result.LastIndexOf( '.' );
                    if(lastPoint != -1) {
                        result = result.Substring( 0, lastPoint );
                    }
                    result += SelectedFilter.Filter;
                }
                return result;
            }
            return ""; // when in directory mode, there is no filename
        }

        private void SetDefaultFileName( string filename ) {
            DefaultFileName = filename;
            FileNameBuffer = filename;
        }

        public void SetPath(string path) {
            ShowDrives = false;
            CurrentPath = path;
            Files.Clear();
            PathDecomposition.Clear();
            if( IsDirectoryMode() ) {
                SetDefaultFileName( "." );
            }
            ScanDir( CurrentPath );
        }

        public void SetCurrentDir(string path) {
            var dir = new DirectoryInfo( path );
            CurrentPath = dir.FullName;
            if(CurrentPath[CurrentPath.Length - 1] == Path.DirectorySeparatorChar) { // handle selecting a drive, like C: -> C:\
                CurrentPath = CurrentPath.Substring( 0, CurrentPath.Length - 1 );
            }

            PathInputBuffer = CurrentPath;
            PathDecomposition = new List<string>( CurrentPath.Split( Path.DirectorySeparatorChar ) );
        }

        private bool IsDirectoryMode() {
            return Filters.Count == 0;
        }

        private void ResetEvents() {
            DrivesClicked = false;
            PathClicked = false;
        }
    }
}
