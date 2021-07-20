using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ImGuiNET;

namespace FDialog {
    // TODO: icons

    public partial class FileDialog {
        // https://github.com/aiekick/ImGuiFileDialog/blob/7b96209e5e96caff9cfeaf3e67dc7cb7d686ae74/ImGuiFileDialog.cpp
        
        // TODO: save path
        // TODO: save filter

        private string Id;
        private string Title;
        private int SelectionCountMax;
        private ImGuiFileDialogFlags Flags;
        // TODO: UserData?

        private string CurrentPath;

        private string DefaultExtension;
        private string DefaultFileName;
        private string FileNameBuffer = "";

        private List<string> PathDecomposition = new();

        private bool Visible;

        private bool IsModal = false;
        private bool OkResultToConfirm = false;

        private bool IsOk;

        private bool CanWeContinue;

        private bool WantsToQuit;

        private bool ShowDrives = false;
        private bool DrivesClicked = false;

        private bool AnyWindowsHovered;

        private bool CreateDirectoryMode = false;
        private string CreateDirectoryBuffer = "";

        private bool PathClicked = true;
        private bool PathInputActivated = false;
        private string PathInputBuffer = "";

        private string SearchBuffer = "";
        private string SearchTag = "";

        private string LastSelectedFileName = "";

        private float FooterHeight = 0;

        private List<string> SelectedFileNames = new();


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

        private void ResetEvents() {
            DrivesClicked = false;
            PathClicked = false;
            CanWeContinue = true;
        }

        private string GetFilePathName() {
            var result = GetCurrentPath();
            var fileName = GetCurrentFileName();

            if(!string.IsNullOrEmpty(fileName)) {
                result = Path.Combine( result, fileName );
            }

            return result;
        }

        private string GetCurrentPath() {
            var path = CurrentPath;
            if(IsDirectoryMode()) {
                var selectedDirectory = FileNameBuffer;
                if(!string.IsNullOrEmpty(selectedDirectory) && selectedDirectory != ".") {
                    if(string.IsNullOrEmpty(path)) {
                        path = selectedDirectory;
                    }
                    else {
                        path = Path.Combine( path, selectedDirectory );
                    }
                }
            }
            return path;
        }

        private string GetCurrentFileName() {
            if( !IsDirectoryMode() ) {
                var result = FileNameBuffer;

                if( SelectedFilter.CollectionFilters != null && SelectedFilter.CollectionFilters.Count > 0 ) {
                    return result;
                }

                // not a collection
                if( !SelectedFilter.Filter.Contains( '*' ) && result != SelectedFilter.Filter ) {
                    var lastPoint = result.LastIndexOf( '.' );
                    if(lastPoint != -1) {
                        result = result.Substring( 0, lastPoint );
                    }
                    result += SelectedFilter.Filter;
                }

                return result;
            }
            return "";
        }

        private void SetDefaultFileName( string filename ) {
            DefaultFileName = filename;
            FileNameBuffer = filename;
        }

        private void SetPath(string path) {
            ShowDrives = false;
            CurrentPath = path;
            Files.Clear();
            PathDecomposition.Clear();
            if( IsDirectoryMode() ) {
                SetDefaultFileName( "." );
            }
            ScanDir( CurrentPath );
        }

        private void SetCurrentDir(string path) {
            var dir = new DirectoryInfo( path );
            CurrentPath = dir.FullName;
            if(CurrentPath[CurrentPath.Length - 1] == Path.DirectorySeparatorChar) {
                CurrentPath = CurrentPath.Substring( 0, CurrentPath.Length - 1 );
            }

            PathInputBuffer = CurrentPath;
            PathDecomposition = new List<string>( CurrentPath.Split( Path.DirectorySeparatorChar ) );
        }

        private bool IsDirectoryMode() {
            return Filters.Count == 0;
        }
    }
}
