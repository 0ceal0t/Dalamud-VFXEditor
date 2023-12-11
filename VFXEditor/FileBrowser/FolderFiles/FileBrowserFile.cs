using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.FileBrowser.FolderFiles {
    public enum FilePickerFileType {
        File,
        Directory
    }

    public enum FilePickerFileColor {
        None,
        Folder,
        Code,
        Misc,
        Image,
        Ffxiv,
        Archive,
    }

    public unsafe class FileBrowserFile {
        private static Dictionary<string, (FontAwesomeIcon, FilePickerFileColor)> ICON_MAP;

        public FilePickerFileType Type;
        public string FilePath;
        public string FileName;
        public string Ext = "";
        public long FileSize = 0;
        public string FormattedFileSize = "";
        public string FileModifiedDate = "";

        public bool Draw( bool selected, out bool doubleClicked ) {
            var ret = false;
            doubleClicked = false;

            var iconColor = Type == FilePickerFileType.Directory ?
                (FontAwesomeIcon.Folder, FilePickerFileColor.Folder) :
                ( GetIcon( Ext, out var _iconColor ) ? _iconColor : (FontAwesomeIcon.File, FilePickerFileColor.None) );

            using var text = ImRaii.PushColor( ImGuiCol.Text, selected ? Plugin.Configuration.FileBrowserSelectedColor : ( iconColor.Item2 switch {
                FilePickerFileColor.Folder => Plugin.Configuration.FileBrowserFolderColor,
                FilePickerFileColor.Code => Plugin.Configuration.FileBrowserCodeColor,
                FilePickerFileColor.Misc => Plugin.Configuration.FileBrowserMiscColor,
                FilePickerFileColor.Image => Plugin.Configuration.FileBrowserImageColor,
                FilePickerFileColor.Ffxiv => Plugin.Configuration.FileBrowserFfxivColor,
                FilePickerFileColor.Archive => Plugin.Configuration.FileBrowserArchiveColor,
                _ => *ImGui.GetStyleColorVec4( ImGuiCol.Text )
            } ) );

            ImGui.TableNextRow();

            if( ImGui.TableNextColumn() ) {
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 7 );
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.Text( iconColor.Item1.ToIconString() );
                }

                ImGui.SameLine( 30f );

                if( ImGui.Selectable( FileName, selected, ImGuiSelectableFlags.AllowDoubleClick | ImGuiSelectableFlags.SpanAllColumns ) ) {
                    ret = true;
                    doubleClicked = ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left );
                }
            }
            if( ImGui.TableNextColumn() ) {
                ImGui.Text( Ext );
            }
            if( ImGui.TableNextColumn() ) {
                ImGui.Text( ( Type == FilePickerFileType.File ? FormattedFileSize : "" ) + "  " );
            }
            if( ImGui.TableNextColumn() ) {
                ImGui.Text( FileModifiedDate + "  " );
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 5 );
            }

            return ret;
        }

        // ========= ICONS ===========

        private static bool GetIcon( string ext, out (FontAwesomeIcon, FilePickerFileColor) result ) {
            if( ICON_MAP == null ) {
                ICON_MAP = new();
                AddToIconMap( new string[] {
                    "mp4",
                    "gif",
                    "mov",
                    "avi",
                }, FontAwesomeIcon.FileVideo, FilePickerFileColor.Misc );

                AddToIconMap( new string[] { "pdf" }, FontAwesomeIcon.FilePdf, FilePickerFileColor.Misc );

                AddToIconMap( new string[] {
                    "png",
                    "jpg",
                    "jpeg",
                    "tiff",
                    "dds",
                    "atex",
                    "tex",
                }, FontAwesomeIcon.FileImage, FilePickerFileColor.Image );

                AddToIconMap( new string[] {
                    "cs",
                    "json",
                    "cpp",
                    "h",
                    "py",
                    "xml",
                    "yaml",
                    "js",
                    "html",
                    "css",
                    "ts",
                    "java",
                    "gltf",
                    "glb",
                    "hkx",
                }, FontAwesomeIcon.FileCode, FilePickerFileColor.Code );

                AddToIconMap( new string[] {
                    "zip",
                    "7z",
                    "gz",
                    "tar",
                    "ttmp",
                    "pmp",
                    "vfxedit2",
                    "vfxworkspace",
                }, FontAwesomeIcon.FileArchive, FilePickerFileColor.Archive );

                AddToIconMap( new string[] {
                    "mp3",
                    "m4a",
                    "ogg",
                    "wav",
                }, FontAwesomeIcon.FileAudio, FilePickerFileColor.Misc );

                AddToIconMap( new string[] {
                    "avfx",
                    "tmb",
                    "pap",
                    "uld",
                    "sklb",
                    "skp",
                    "eid",
                    "phyb",
                    "atch",
                    "shcd",
                    "shpk",
                    "sgb",
                }, FontAwesomeIcon.File, FilePickerFileColor.Ffxiv );

                AddToIconMap( new string[] { "csv" }, FontAwesomeIcon.FileCsv, FilePickerFileColor.Misc );
            }

            return ICON_MAP.TryGetValue( ext.ToLower(), out result );
        }

        private static void AddToIconMap( string[] extensions, FontAwesomeIcon icon, FilePickerFileColor color ) {
            foreach( var ext in extensions ) {
                ICON_MAP[ext] = (icon, color);
            }
        }

        // ========= UTILS ==========

        public static FileBrowserFile FromFile( FileInfo file, string path ) => new() {
            FileName = file.Name,
            FilePath = path,
            FileModifiedDate = FormatModifiedDate( file.LastWriteTime ),
            FileSize = file.Length,
            FormattedFileSize = BytesToString( file.Length ),
            Type = FilePickerFileType.File,
            Ext = file.Extension.Trim( '.' )
        };

        public static FileBrowserFile FromDirectory( DirectoryInfo dir, string path ) => new() {
            FileName = dir.Name,
            FilePath = path,
            FileModifiedDate = FormatModifiedDate( dir.LastWriteTime ),
            Type = FilePickerFileType.Directory,
        };

        private static string FormatModifiedDate( DateTime date ) => date.ToString( "yyyy/MM/dd HH:mm" );

        private static string BytesToString( long byteCount ) {
            var suffix = new string[] { " B", " KB", " MB", " GB", " TB" };
            if( byteCount == 0 ) return "0" + suffix[0];
            var bytes = Math.Abs( byteCount );
            var place = Convert.ToInt32( Math.Floor( Math.Log( bytes, 1024 ) ) );
            var num = Math.Round( bytes / Math.Pow( 1024, place ), 1 );
            return ( Math.Sign( byteCount ) * num ).ToString() + suffix[place];
        }

        // ======== SORTING ===========

        public static int SortByFileNameDesc( FileBrowserFile a, FileBrowserFile b ) {
            if( a.FileName[0] == '.' && b.FileName[0] != '.' ) return 1;
            if( a.FileName[0] != '.' && b.FileName[0] == '.' ) return -1;
            if( a.FileName[0] == '.' && b.FileName[0] == '.' ) {
                if( a.FileName.Length == 1 ) return -1;
                if( b.FileName.Length == 1 ) return 1;
                return -1 * string.Compare( a.FileName[1..], b.FileName[1..] );
            }

            if( a.Type != b.Type ) return ( a.Type == FilePickerFileType.Directory ? 1 : -1 );
            return -1 * string.Compare( a.FileName, b.FileName );
        }

        public static int SortByFileNameAsc( FileBrowserFile a, FileBrowserFile b ) {
            if( a.FileName[0] == '.' && b.FileName[0] != '.' ) return -1;
            if( a.FileName[0] != '.' && b.FileName[0] == '.' ) return 1;
            if( a.FileName[0] == '.' && b.FileName[0] == '.' ) {
                if( a.FileName.Length == 1 ) return 1;
                if( b.FileName.Length == 1 ) return -1;
                return string.Compare( a.FileName[1..], b.FileName[1..] );
            }

            if( a.Type != b.Type ) return ( a.Type == FilePickerFileType.Directory ? -1 : 1 );
            return string.Compare( a.FileName, b.FileName );
        }

        public static int SortByTypeDesc( FileBrowserFile a, FileBrowserFile b ) {
            if( a.Type != b.Type ) return ( a.Type == FilePickerFileType.Directory ) ? 1 : -1;
            return string.Compare( a.Ext, b.Ext );
        }

        public static int SortByTypeAsc( FileBrowserFile a, FileBrowserFile b ) {
            if( a.Type != b.Type ) return ( a.Type == FilePickerFileType.Directory ) ? -1 : 1;
            return -1 * string.Compare( a.Ext, b.Ext );
        }

        public static int SortBySizeDesc( FileBrowserFile a, FileBrowserFile b ) {
            if( a.Type != b.Type ) return ( a.Type == FilePickerFileType.Directory ) ? 1 : -1;
            return ( a.FileSize > b.FileSize ) ? 1 : -1;
        }

        public static int SortBySizeAsc( FileBrowserFile a, FileBrowserFile b ) {
            if( a.Type != b.Type ) return ( a.Type == FilePickerFileType.Directory ) ? -1 : 1;
            return ( a.FileSize > b.FileSize ) ? -1 : 1;
        }

        public static int SortByDateDesc( FileBrowserFile a, FileBrowserFile b ) {
            if( a.Type != b.Type ) return ( a.Type == FilePickerFileType.Directory ) ? 1 : -1;
            return string.Compare( a.FileModifiedDate, b.FileModifiedDate );
        }

        public static int SortByDateAsc( FileBrowserFile a, FileBrowserFile b ) {
            if( a.Type != b.Type ) return ( a.Type == FilePickerFileType.Directory ) ? -1 : 1;
            return -1 * string.Compare( a.FileModifiedDate, b.FileModifiedDate );
        }
    }
}
