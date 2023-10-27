using Dalamud.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FilePicker.SideBar;

namespace VfxEditor.FilePicker {
    public static class FilePickerManager {
        private static FilePickerDialog Dialog;
        private static string SavedPath = ".";
        private static Action<bool, string> Callback;
        private static readonly List<FilePickerSidebarItem> Recent = new();

        public static void Dispose() {
            UnloadDialog();
            Dialog = null;
            Callback = null;
            Recent.Clear();
        }

        public static void OpenFolderDialog( string title, Action<bool, string> callback ) {
            SetDialog( "OpenFolderDialog", title, "", ".", "", false, ImGuiFileDialogFlags.SelectOnly, true, callback );
        }

        public static void SaveFolderDialog( string title, string defaultFolderName, Action<bool, string> callback ) {
            SetDialog( "SaveFolderDialog", title, "", defaultFolderName, "", false, ImGuiFileDialogFlags.None, true, callback );
        }

        public static void OpenFileDialog( string title, string filters, Action<bool, string> callback ) {
            SetDialog( "OpenFileDialog", title, filters, ".", "", false, ImGuiFileDialogFlags.SelectOnly, false, callback );
        }

        public static void OpenFileModal( string title, string filters, Action<bool, string> callback ) {
            SetDialog( "OpenFileDialog", title, filters, ".", "", true, ImGuiFileDialogFlags.SelectOnly, false, callback );
        }

        public static void SaveFileDialog( string title, string filters, string defaultFileName, string defaultExtension, Action<bool, string> callback ) {
            SetDialog( "SaveFileDialog", title, filters, defaultFileName, defaultExtension, false, ImGuiFileDialogFlags.ConfirmOverwrite, false, callback );
        }

        private static void SetDialog(
            string id,
            string title,
            string filters,
            string defaultFileName,
            string defaultExtension,
            bool modal,
            ImGuiFileDialogFlags flags,
            bool folderDialog,
            Action<bool, string> callback
        ) {
            UnloadDialog();
            Callback = callback;
            Dialog = new FilePickerDialog( id, title, modal, flags, folderDialog, filters, SavedPath, defaultFileName, defaultExtension, Recent );
            Dialog.Show();
        }

        public static void Draw() {
            if( Dialog == null ) return;
            if( Dialog.Draw() ) {
                Callback( Dialog.GetIsOk(), Dialog.GetResult() );
                SavedPath = Dialog.GetCurrentPath();
                AddRecent( SavedPath );
                UnloadDialog();
                Dialog = null;
                Callback = null;
            }
        }

        private static void UnloadDialog() {
            Dialog?.Hide();
        }

        private static void AddRecent( string path ) {
            foreach( var recent in Recent ) {
                if( recent.Location == path ) return;
            }

            Recent.Add( new FilePickerSidebarItem {
                Icon = FontAwesomeIcon.Folder,
                Location = path,
                Text = Path.GetFileName( path )
            } );

            while( Recent.Count > 10 ) {
                Recent.RemoveAt( 0 );
            }
        }
    }
}
