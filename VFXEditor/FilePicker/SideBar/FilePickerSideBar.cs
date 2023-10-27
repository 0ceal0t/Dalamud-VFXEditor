using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VfxEditor.Utils;

namespace VfxEditor.FilePicker.SideBar {
    public class FilePickerSideBar {
        private readonly FilePickerDialog Dialog;

        private FilePickerSidebarItem Selected;

        private readonly List<FilePickerSidebarItem> Drives = new();
        private readonly List<FilePickerSidebarItem> QuickAccess = new();
        private readonly List<FilePickerSidebarItem> Favorites = new();
        private readonly List<FilePickerSidebarItem> Recent;

        public FilePickerSideBar( FilePickerDialog dialog, List<FilePickerSidebarItem> recent ) {
            Dialog = dialog;
            Recent = recent;

            foreach( var drive in DriveInfo.GetDrives() ) {
                Drives.Add( new FilePickerSidebarItem {
                    Icon = FontAwesomeIcon.Server,
                    Location = drive.Name,
                    Text = drive.Name
                } );
            }

            var personal = Path.GetDirectoryName( Environment.GetFolderPath( Environment.SpecialFolder.Personal ) );
            if( string.IsNullOrEmpty( personal ) ) return;
            if( personal.EndsWith( "OneDrive" ) ) personal = personal.Replace( "OneDrive", "" ); // >:(

            QuickAccess.Add( new FilePickerSidebarItem {
                Icon = FontAwesomeIcon.Desktop,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.Desktop ),
                Text = "Desktop"
            } );

            QuickAccess.Add( new FilePickerSidebarItem {
                Icon = FontAwesomeIcon.File,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ),
                Text = "Documents"
            } );

            QuickAccess.Add( new FilePickerSidebarItem {
                Icon = FontAwesomeIcon.Download,
                Location = Path.Combine( personal, "Downloads" ),
                Text = "Downloads"
            } );

            QuickAccess.Add( new FilePickerSidebarItem {
                Icon = FontAwesomeIcon.Star,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.Favorites ),
                Text = "Favorites"
            } );

            QuickAccess.Add( new FilePickerSidebarItem {
                Icon = FontAwesomeIcon.Music,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ),
                Text = "Music"
            } );

            QuickAccess.Add( new FilePickerSidebarItem {
                Icon = FontAwesomeIcon.Image,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.MyPictures ),
                Text = "Pictures"
            } );

            QuickAccess.Add( new FilePickerSidebarItem {
                Icon = FontAwesomeIcon.Video,
                Location = Environment.GetFolderPath( Environment.SpecialFolder.MyVideos ),
                Text = "Videos"
            } );

            if( GetQuickAccessFolders( out var folders ) ) {
                foreach( var (name, path) in folders ) {
                    Favorites.Add( new FilePickerSidebarItem {
                        Icon = FontAwesomeIcon.Folder,
                        Location = path,
                        Text = $"{name}"
                    } );
                }
            }
        }

        public void ClearSelected() {
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

        private void DrawCategory( List<FilePickerSidebarItem> items, string name, FontAwesomeIcon icon, ref int idx ) {
            if( items == null || items.Count == 0 ) return;

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

        private void DrawSideBarItem( FilePickerSidebarItem item, ref int idx ) {
            using var _ = ImRaii.PushId( idx );

            if( item.Draw( item == Selected ) ) {
                Dialog.SetPath( item.Location );
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
