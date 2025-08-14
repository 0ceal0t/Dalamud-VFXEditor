using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;

namespace VfxEditor.FileBrowser.SideBar {
    public class FileBrowserSideBar {
        private readonly FileBrowserDialog Dialog;

        private FileBrowserSidebarItem Selected;

        private readonly ConcurrentQueue<FileBrowserSidebarItem> Drives = new();
        private readonly ConcurrentQueue<FileBrowserSidebarItem> QuickAccess = new();
        private readonly ConcurrentQueue<FileBrowserSidebarItem> Pinned = new();

        public FileBrowserSideBar( FileBrowserDialog dialog ) {
            Dialog = dialog;
            PopulateDrives();
            PopulateQuickAccess();
            PopulatePinned();
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

        private async void PopulatePinned() {
            await Task.Run( () => {
                if( GetPinnedFolders( out var folders ) ) {
                    foreach( var (name, path) in folders ) {
                        Pinned.Enqueue( new FileBrowserSidebarItem {
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

            DrawCategory( Drives, "Drives", FontAwesomeIcon.Display, ref idx );
            DrawCategory( QuickAccess, "Quick Access", FontAwesomeIcon.Home, ref idx );
            DrawCategory( Pinned, "Pinned", FontAwesomeIcon.Thumbtack, ref idx );
            DrawCategory( Plugin.Configuration.FileBrowserFavorites, "Favorites", FontAwesomeIcon.Star, ref idx );
            DrawCategory( Plugin.Configuration.FileBrowserRecent, "Recent", FontAwesomeIcon.History, ref idx );
        }

        private unsafe void DrawCategory( IEnumerable<FileBrowserSidebarItem> items, string name, FontAwesomeIcon icon, ref int idx ) {
            using var indent = ImRaii.PushStyle( ImGuiStyleVar.IndentSpacing, 15f );
            using var framePadding = ImRaii.PushStyle( ImGuiStyleVar.FramePadding, new Vector2( 0, 0 ) );

            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            using var disabled = ImRaii.PushColor( ImGuiCol.Text, *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ) );
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            using var tree = ImRaii.TreeNode( icon.ToIconString(), ImGuiTreeNodeFlags.DefaultOpen );
            font.Dispose();
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) { ImGui.SameLine(); }
            ImGui.Text( name );
            disabled.Dispose();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( !tree ) return;
            if( items == null ) return;
            foreach( var item in items ) {
                using var _ = ImRaii.PushId( idx++ );

                if( item.Draw( item == Selected ) ) {
                    Dialog.SetPath( item.Location, true );
                    Selected = item;
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
        }

        private static bool GetPinnedFolders( out List<(string Name, string Path)> folders ) {
            folders = [];
            try {
                var shellAppType = Type.GetTypeFromProgID( "Shell.Application" );
                if( shellAppType == null ) return false;

                var shell = Activator.CreateInstance( shellAppType );
                var obj = shellAppType.InvokeMember( "NameSpace", BindingFlags.InvokeMethod, null, shell,
                [
                "shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}",
                ] );
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
