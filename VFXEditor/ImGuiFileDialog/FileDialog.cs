using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImGuiFileDialog {
    [Flags]
    public enum ImGuiFileDialogFlags {
        None = 0,
        ConfirmOverwrite = 1,
        SelectOnly = 2,
        DontShowHiddenFiles = 4,
        DisableCreateDirectoryButton = 8,
        HideSideBar = 16,
        LoadPreview = 32,
    }

    public class SideBarItem {
        public char Icon;
        public string Text;
        public string Location;
    }

    public partial class FileDialog {
        private bool Visible;
        private readonly DalamudPluginInterface PluginInterface;

        private readonly string Title;
        private readonly int SelectionCountMax;
        private readonly ImGuiFileDialogFlags Flags;
        private readonly string Id;
        private readonly string DefaultExtension;
        private readonly string DefaultFileName;

        private string CurrentPath;
        private string FileNameBuffer = "";

        private List<string> PathDecomposition = new();
        private bool PathClicked = true;
        private bool PathInputActivated = false;
        private string PathInputBuffer = "";

        private readonly bool IsModal = false;
        private bool OkResultToConfirm = false;
        private bool IsOk;
        private bool WantsToQuit;

        private bool CreateDirectoryMode = false;
        private string CreateDirectoryBuffer = "";

        private string SearchBuffer = "";

        private string LastSelectedFileName = "";
        private readonly List<string> SelectedFileNames = new();

        private float FooterHeight = 0;

        private SideBarItem SelectedSideBar = null;
        private readonly List<SideBarItem> Drives = new();
        private readonly List<SideBarItem> QuickAccess = new();
        private readonly List<SideBarItem> Favorites = new();
        private readonly List<SideBarItem> Recent;

        private ImGuiScene.TextureWrap PreviewWrap = null;
        private readonly object PreviewLock = new();
        private readonly bool DoLoadPreview;

        public FileDialog(
            DalamudPluginInterface pluginInterface,
            string id,
            string title,
            string filters,
            string path,
            string defaultFileName,
            string defaultExtension,
            int selectionCountMax,
            bool isModal,
            List<SideBarItem> recent,
            ImGuiFileDialogFlags flags
         ) {
            PluginInterface = pluginInterface;
            Id = id;
            Title = title;
            Flags = flags;
            SelectionCountMax = selectionCountMax;
            IsModal = isModal;
            Recent = recent;
            DoLoadPreview = Flags.HasFlag( ImGuiFileDialogFlags.LoadPreview );

            CurrentPath = path;
            DefaultExtension = defaultExtension;
            DefaultFileName = defaultFileName;

            ParseFilters( filters );
            SetSelectedFilterWithExt( DefaultExtension );
            SetDefaultFileName();
            SetPath( CurrentPath );

            SetupSideBar();
        }

        public void Show() {
            Visible = true;
        }

        public void Hide() {
            Visible = false;
        }

        public bool GetIsOk() => IsOk;

        public string GetResult() {
            if( !Flags.HasFlag( ImGuiFileDialogFlags.SelectOnly ) ) return GetFilePathName();
            if( IsDirectoryMode() && SelectedFileNames.Count == 0 ) {
                return GetFilePathName(); // current directory
            }

            var fullPaths = SelectedFileNames.Where( x => !string.IsNullOrEmpty( x ) ).Select( x => Path.Combine( CurrentPath, x ) );
            return string.Join( ",", fullPaths.ToArray() );
        }

        public void Dispose() => PreviewWrap?.Dispose();

        // the full path, specified by the text input box and the current path
        private string GetFilePathName() {
            var path = GetCurrentPath();
            var fileName = GetCurrentFileName();

            if( !string.IsNullOrEmpty( fileName ) ) return Path.Combine( path, fileName );
            return path;
        }

        // the current path. In directory mode, this takes into account the text input box
        public string GetCurrentPath() {
            if( IsDirectoryMode() ) { // combine path file with directory input
                var selectedDirectory = FileNameBuffer;
                if( !string.IsNullOrEmpty( selectedDirectory ) && selectedDirectory != "." ) {
                    return string.IsNullOrEmpty( CurrentPath ) ? selectedDirectory : Path.Combine( CurrentPath, selectedDirectory );
                }
            }

            return CurrentPath;
        }

        // the current filename, taking into account the current filter applied. In directory mod, this is alway empty
        private string GetCurrentFileName() {
            if( IsDirectoryMode() ) return "";

            var result = FileNameBuffer;
            // a collection like {.cpp, .h}, so can't decide on an extension
            if( SelectedFilter.CollectionFilters != null && SelectedFilter.CollectionFilters.Count > 0 ) return result;

            // a single one, like .cpp
            if( !SelectedFilter.Filter.Contains( '*' ) && result != SelectedFilter.Filter ) {
                var lastPoint = result.LastIndexOf( '.' );
                if( lastPoint != -1 ) {
                    result = result[..lastPoint];
                }
                result += SelectedFilter.Filter;
            }
            return result;
        }

        private void SetDefaultFileName() {
            FileNameBuffer = DefaultFileName;
        }

        private void SetPath( string path ) {
            SearchBuffer = string.Empty;
            SelectedSideBar = null;
            CurrentPath = path;
            Files.Clear();
            PathDecomposition.Clear();
            SelectedFileNames.Clear();
            if( IsDirectoryMode() ) {
                SetDefaultFileName();
            }
            ScanDir( CurrentPath );
        }

        private void SetCurrentDir( string path ) {
            var dir = new DirectoryInfo( path );
            CurrentPath = dir.FullName;
            if( CurrentPath[^1] == Path.DirectorySeparatorChar ) { // handle selecting a drive, like C: -> C:\
                CurrentPath = CurrentPath[0..^1];
            }

            PathInputBuffer = CurrentPath;
            PathDecomposition = new List<string>( CurrentPath.Split( Path.DirectorySeparatorChar ) );
        }

        private bool IsDirectoryMode() => Filters.Count == 0;

        private void ResetEvents() {
            PathClicked = false;
        }
    }
}
