using Dalamud.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ImGuiFileDialog {
    public partial class FileDialog {
        private enum FileStructType {
            File,
            Directory
        }

        private enum SortingField {
            None,
            FileName,
            Type,
            Size,
            Date
        }

        private struct FileStruct {
            public FileStructType Type;
            public string FilePath;
            public string FileName;
            public string Ext;
            public long FileSize;
            public string FormattedFileSize;
            public string FileModifiedDate;
        }

        private readonly List<FileStruct> Files = new();
        private readonly List<FileStruct> FilteredFiles = new();

        private readonly object FilesLock = new();

        private SortingField CurrentSortingField = SortingField.FileName;
        private readonly bool[] SortDescending = [false, false, false, false];

        private bool CreateDir( string dirPath ) {
            var newPath = Path.Combine( CurrentPath, dirPath );
            if( string.IsNullOrEmpty( newPath ) ) return false;

            Directory.CreateDirectory( newPath );
            return true;
        }

        private static string ComposeNewPath( List<string> decomp ) {
            if( decomp.Count == 1 ) {
                var drivePath = decomp[0];
                if( drivePath[^1] != Path.DirectorySeparatorChar ) { // turn C: into C:\
                    drivePath += Path.DirectorySeparatorChar;
                }
                return drivePath;
            }
            return Path.Combine( decomp.ToArray() );
        }

        private void ScanDir( string path ) {
            if( !Directory.Exists( path ) ) return;

            if( PathDecomposition.Count == 0 ) {
                SetCurrentDir( path );
            }

            if( PathDecomposition.Count > 0 ) {
                Files.Clear();

                if( PathDecomposition.Count > 1 ) {
                    Files.Add( new FileStruct {
                        Type = FileStructType.Directory,
                        FilePath = path,
                        FileName = "..",
                        FileSize = 0,
                        FileModifiedDate = "",
                        FormattedFileSize = "",
                        Ext = ""
                    } );
                }

                var dirInfo = new DirectoryInfo( path );

                var dontShowHidden = Flags.HasFlag( ImGuiFileDialogFlags.DontShowHiddenFiles );

                foreach( var dir in dirInfo.EnumerateDirectories().OrderBy( d => d.Name ) ) {
                    if( string.IsNullOrEmpty( dir.Name ) ) continue;
                    if( dontShowHidden && dir.Name[0] == '.' ) continue;

                    Files.Add( GetDir( dir, path ) );
                }

                foreach( var file in dirInfo.EnumerateFiles().OrderBy( f => f.Name ) ) {
                    if( string.IsNullOrEmpty( file.Name ) ) continue;
                    if( dontShowHidden && file.Name[0] == '.' ) continue;

                    if( !string.IsNullOrEmpty( file.Extension ) ) {
                        var ext = file.Extension;
                        if( Filters.Count > 0 && !SelectedFilter.Empty() && !SelectedFilter.FilterExists( ext.ToLower() ) && SelectedFilter.Filter != ".*" ) continue;
                    }

                    Files.Add( GetFile( file, path ) );
                }

                SortFields( CurrentSortingField );
            }
        }

        private static FileStruct GetFile( FileInfo file, string path ) {
            return new FileStruct {
                FileName = file.Name,
                FilePath = path,
                FileModifiedDate = FormatModifiedDate( file.LastWriteTime ),
                FileSize = file.Length,
                FormattedFileSize = BytesToString( file.Length ),
                Type = FileStructType.File,
                Ext = file.Extension.Trim( '.' )
            };
        }

        private static FileStruct GetDir( DirectoryInfo dir, string path ) {
            return new FileStruct {
                FileName = dir.Name,
                FilePath = path,
                FileModifiedDate = FormatModifiedDate( dir.LastWriteTime ),
                FileSize = 0,
                FormattedFileSize = "",
                Type = FileStructType.Directory,
                Ext = ""
            };
        }

        private void SetupSideBar() {
            var drives = DriveInfo.GetDrives();
            foreach( var drive in drives ) {
                Drives.Add( new SideBarItem {
                    Icon = ( char )FontAwesomeIcon.Server,
                    Location = drive.Name,
                    Text = drive.Name
                } );
            }

            var personal = Path.GetDirectoryName( Environment.GetFolderPath( Environment.SpecialFolder.Personal ) );
            if( string.IsNullOrEmpty( personal ) ) return;
            if( personal.EndsWith( "OneDrive" ) ) personal = personal.Replace( "OneDrive", "" ); // >:(

            QuickAccess.Add( new SideBarItem {
                Icon = ( char )FontAwesomeIcon.Desktop,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.Desktop ),
                Text = "Desktop"
            } );

            QuickAccess.Add( new SideBarItem {
                Icon = ( char )FontAwesomeIcon.File,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ),
                Text = "Documents"
            } );

            QuickAccess.Add( new SideBarItem {
                Icon = ( char )FontAwesomeIcon.Download,
                Location = Path.Combine( personal, "Downloads" ),
                Text = "Downloads"
            } );

            QuickAccess.Add( new SideBarItem {
                Icon = ( char )FontAwesomeIcon.Star,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.Favorites ),
                Text = "Favorites"
            } );

            QuickAccess.Add( new SideBarItem {
                Icon = ( char )FontAwesomeIcon.Music,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ),
                Text = "Music"
            } );

            QuickAccess.Add( new SideBarItem {
                Icon = ( char )FontAwesomeIcon.Image,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.MyPictures ),
                Text = "Pictures"
            } );

            QuickAccess.Add( new SideBarItem {
                Icon = ( char )FontAwesomeIcon.Video,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.MyVideos ),
                Text = "Videos"
            } );

            if( GetQuickAccessFolders( out var folders ) ) {
                for( var idx = 0; idx < folders.Count; idx++ ) {
                    var (name, path) = folders[idx];
                    Favorites.Add( new SideBarItem {
                        Icon = ( char )FontAwesomeIcon.Folder,
                        Location = path,
                        Text = $"{name}"
                    } );
                }
            }
        }

        private static bool GetQuickAccessFolders( out List<(string Name, string Path)> folders ) {
            folders = [];
            try {
                var shellAppType = Type.GetTypeFromProgID( "Shell.Application" );
                if( shellAppType == null )
                    return false;

                var shell = Activator.CreateInstance( shellAppType );

                var obj = shellAppType.InvokeMember( "NameSpace", BindingFlags.InvokeMethod, null, shell, new object[]
                {
                "shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}",
                } );
                if( obj == null )
                    return false;


                foreach( var fi in ( ( dynamic )obj ).Items() ) {
                    if( !fi.IsLink && !fi.IsFolder )
                        continue;

                    folders.Add( (fi.Name, fi.Path) );
                }

                return true;
            }
            catch {
                return false;
            }
        }

        // TODO: ascending or descending icons

        private void SortFields( SortingField sortingField, bool canChangeOrder = false ) {
            switch( sortingField ) {
                case SortingField.FileName:
                    if( canChangeOrder && sortingField == CurrentSortingField ) {
                        SortDescending[0] = !SortDescending[0];
                    }
                    Files.Sort( SortDescending[0] ? SortByFileNameDesc : SortByFileNameAsc );
                    break;

                case SortingField.Type:
                    if( canChangeOrder && sortingField == CurrentSortingField ) {
                        SortDescending[1] = !SortDescending[1];
                    }
                    Files.Sort( SortDescending[1] ? SortByTypeDesc : SortByTypeAsc );
                    break;

                case SortingField.Size:
                    if( canChangeOrder && sortingField == CurrentSortingField ) {
                        SortDescending[2] = !SortDescending[2];
                    }
                    Files.Sort( SortDescending[2] ? SortBySizeDesc : SortBySizeAsc );
                    break;

                case SortingField.Date:
                    if( canChangeOrder && sortingField == CurrentSortingField ) {
                        SortDescending[3] = !SortDescending[3];
                    }
                    Files.Sort( SortDescending[3] ? SortByDateDesc : SortByDateAsc );
                    break;
            }

            if( sortingField != SortingField.None ) {
                CurrentSortingField = sortingField;
            }

            ApplyFilteringOnFileList();
        }

        private static int SortByFileNameDesc( FileStruct a, FileStruct b ) {
            if( a.FileName[0] == '.' && b.FileName[0] != '.' ) return 1;
            if( a.FileName[0] != '.' && b.FileName[0] == '.' ) return -1;
            if( a.FileName[0] == '.' && b.FileName[0] == '.' ) {
                if( a.FileName.Length == 1 ) return -1;
                if( b.FileName.Length == 1 ) return 1;
                return -1 * string.Compare( a.FileName[1..], b.FileName[1..] );
            }

            if( a.Type != b.Type ) return ( a.Type == FileStructType.Directory ? 1 : -1 );
            return -1 * string.Compare( a.FileName, b.FileName );
        }

        private static int SortByFileNameAsc( FileStruct a, FileStruct b ) {
            if( a.FileName[0] == '.' && b.FileName[0] != '.' ) return -1;
            if( a.FileName[0] != '.' && b.FileName[0] == '.' ) return 1;
            if( a.FileName[0] == '.' && b.FileName[0] == '.' ) {
                if( a.FileName.Length == 1 ) return 1;
                if( b.FileName.Length == 1 ) return -1;
                return string.Compare( a.FileName[1..], b.FileName[1..] );
            }

            if( a.Type != b.Type ) return ( a.Type == FileStructType.Directory ? -1 : 1 );
            return string.Compare( a.FileName, b.FileName );
        }

        private static int SortByTypeDesc( FileStruct a, FileStruct b ) {
            if( a.Type != b.Type ) return ( a.Type == FileStructType.Directory ) ? 1 : -1;
            return string.Compare( a.Ext, b.Ext );
        }

        private static int SortByTypeAsc( FileStruct a, FileStruct b ) {
            if( a.Type != b.Type ) return ( a.Type == FileStructType.Directory ) ? -1 : 1;
            return -1 * string.Compare( a.Ext, b.Ext );
        }

        private static int SortBySizeDesc( FileStruct a, FileStruct b ) {
            if( a.Type != b.Type ) return ( a.Type == FileStructType.Directory ) ? 1 : -1;
            return ( a.FileSize > b.FileSize ) ? 1 : -1;
        }

        private static int SortBySizeAsc( FileStruct a, FileStruct b ) {
            if( a.Type != b.Type ) return ( a.Type == FileStructType.Directory ) ? -1 : 1;
            return ( a.FileSize > b.FileSize ) ? -1 : 1;
        }

        private static int SortByDateDesc( FileStruct a, FileStruct b ) {
            if( a.Type != b.Type ) return ( a.Type == FileStructType.Directory ) ? 1 : -1;
            return string.Compare( a.FileModifiedDate, b.FileModifiedDate );
        }

        private static int SortByDateAsc( FileStruct a, FileStruct b ) {
            if( a.Type != b.Type ) return ( a.Type == FileStructType.Directory ) ? -1 : 1;
            return -1 * string.Compare( a.FileModifiedDate, b.FileModifiedDate );
        }
    }
}
