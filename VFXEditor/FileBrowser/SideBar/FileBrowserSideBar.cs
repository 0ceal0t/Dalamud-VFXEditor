using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VfxEditor.Utils;

namespace VfxEditor.FileBrowser.SideBar {
    public class FileBrowserSideBar {
        private readonly FileBrowserDialog Dialog;

        private FileBrowserSidebarItem Selected;

        private readonly ConcurrentQueue<FileBrowserSidebarItem> Drives = new();
        private readonly ConcurrentQueue<FileBrowserSidebarItem> QuickAccess = new();
        private readonly ConcurrentQueue<FileBrowserSidebarItem> Favorites = new();
        private readonly List<FileBrowserSidebarItem> Recent;

        public FileBrowserSideBar( FileBrowserDialog dialog, List<FileBrowserSidebarItem> recent ) {
            Dialog = dialog;
            Recent = recent;

            PopulateDrives();
            PopulateQuickAccess();
            PopulateFavorites();
        }

        private async void PopulateDrives() {
            await Task.Run( () => {
                foreach( var drive in DriveInfo.GetDrives() ) {
                    if( !drive.IsReady ) continue;
                    var location = drive.Name;
                    if( location[^1] == Path.DirectorySeparatorChar ) location = location[0..^1];
                    var label = drive.VolumeLabel;
                    var text = string.IsNullOrEmpty( label ) ? location : $"{label} ({location})";

                    Drives.Enqueue( new FileBrowserSidebarItem {
                        Icon = FontAwesomeIcon.Server,
                        Location = drive.Name,
                        Text = text,
                    } );
                }
            } );
        }

        private async void PopulateQuickAccess() {
            await Task.Run( () => {
                var personal = Path.GetDirectoryName( Environment.GetFolderPath( Environment.SpecialFolder.Personal ) );
                if( string.IsNullOrEmpty( personal ) ) return;
                if( personal.EndsWith( "OneDrive" ) ) personal = personal.Replace( "OneDrive", "" ); // >:(

                QuickAccess.Enqueue( new FileBrowserSidebarItem {
                    Icon = FontAwesomeIcon.Desktop,
                    Location = Environment.GetFolderPath( Environment.SpecialFolder.Desktop ),
                    Text = "Desktop"
                } );

                QuickAccess.Enqueue( new FileBrowserSidebarItem {
                    Icon = FontAwesomeIcon.File,
                    Location = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ),
                    Text = "Documents"
                } );

                QuickAccess.Enqueue( new FileBrowserSidebarItem {
                    Icon = FontAwesomeIcon.Download,
                    Location = Path.Combine( personal, "Downloads" ),
                    Text = "Downloads"
                } );

                QuickAccess.Enqueue( new FileBrowserSidebarItem {
                    Icon = FontAwesomeIcon.Star,
                    Location = Environment.GetFolderPath( Environment.SpecialFolder.Favorites ),
                    Text = "Favorites"
                } );

                QuickAccess.Enqueue( new FileBrowserSidebarItem {
                    Icon = FontAwesomeIcon.Music,
                    Location = Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ),
                    Text = "Music"
                } );

                QuickAccess.Enqueue( new FileBrowserSidebarItem {
                    Icon = FontAwesomeIcon.Image,
                    Location = Environment.GetFolderPath( Environment.SpecialFolder.MyPictures ),
                    Text = "Pictures"
                } );

                QuickAccess.Enqueue( new FileBrowserSidebarItem {
                    Icon = FontAwesomeIcon.Video,
                    Location = Environment.GetFolderPath( Environment.SpecialFolder.MyVideos ),
                    Text = "Videos"
                } );
            } );
        }

        private async void PopulateFavorites() {
            await Task.Run( () => {
                if( GetQuickAccessFolders( out var folders ) ) {
                    foreach( var (name, path) in folders ) {
                        Favorites.Enqueue( new FileBrowserSidebarItem {
                            Icon = FontAwesomeIcon.Folder,
                            Location = path,
                            Text = $"{name}"
                        } );
                    }
                }
            } );
        }

        public void Clear() {
            Selected = null;
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "SideBar" );
            using var child = ImRaii.Child( "Child" );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            var idx = 0;

            foreach( var item in Drives ) DrawSideBarItem( item, ref idx );
            foreach( var item in QuickAccess ) DrawSideBarItem( item, ref idx );
            DrawCategory( Favorites, "Favorites", FontAwesomeIcon.Star, ref idx );
            DrawCategory( Recent, "Recent", FontAwesomeIcon.History, ref idx );
        }

        private void DrawCategory( IEnumerable<FileBrowserSidebarItem> items, string name, FontAwesomeIcon icon, ref int idx ) {
            if( items == null || items.Count() == 0 ) return;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            using( var disabled = ImRaii.Disabled() ) {
                UiUtils.IconText( icon );
                ImGui.SameLine();
                ImGui.Text( name );
            }

            foreach( var item in items ) {
                DrawSideBarItem( item, ref idx );
            }
        }

        private void DrawSideBarItem( FileBrowserSidebarItem item, ref int idx ) {
            using var _ = ImRaii.PushId( idx );

            if( item.Draw( item == Selected ) ) {
                Dialog.SetPath( item.Location, true );
                Selected = item;
            }

            idx++;
        }

        private static bool GetQuickAccessFolders( out List<(string Name, string Path)> folders ) {
            folders = new();
            try {
                var shellAppType = Type.GetTypeFromProgID( "Shell.Application" );
                if( shellAppType == null ) return false;

                var shell = Activator.CreateInstance( shellAppType );
                var obj = shellAppType.InvokeMember( "NameSpace", BindingFlags.InvokeMethod, null, shell, new object[]
                {
                "shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}",
                } );
                if( obj == null ) return false;


                foreach( var item in ( ( dynamic )obj ).Items() ) {
                    if( !item.IsLink && !item.IsFolder ) continue;

                    folders.Add( (item.Name, item.Path) );
                }

                return true;
            }
            catch {
                return false;
            }
        }
    }
}
