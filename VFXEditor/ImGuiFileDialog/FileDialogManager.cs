using Dalamud.Interface;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;

namespace ImGuiFileDialog {
    public static class FileDialogManager {
        private static FileDialog Dialog;
        private static string SavedPath;
        private static Action<bool, string> Callback;
        private static List<SideBarItem> Recent;
        private static DalamudPluginInterface PluginInterface;

        public static bool ImagePreview { get; set; } = true;

        public static void Initialize( DalamudPluginInterface pluginInterface ) {
            PluginInterface = pluginInterface;
            Dialog = null;
            SavedPath = ".";
            Callback = null;
            Recent = new();
        }

        public static void Dispose() {
            UnloadDialog();
            Dialog = null;
            Callback = null;
            Recent = null;
            SavedPath = null;
            PluginInterface = null;
        }

        public static void OpenFolderDialog( string title, Action<bool, string> callback ) {
            SetDialog( "OpenFolderDialog", title, "", SavedPath, ".", "", 1, false, ImGuiFileDialogFlags.SelectOnly, callback );
        }

        public static void SaveFolderDialog( string title, string defaultFolderName, Action<bool, string> callback ) {
            SetDialog( "SaveFolderDialog", title, "", SavedPath, defaultFolderName, "", 1, false, ImGuiFileDialogFlags.None, callback );
        }

        public static void OpenFileDialog( string title, string filters, Action<bool, string> callback ) {
            SetDialog( "OpenFileDialog", title, filters, SavedPath, ".", "", 1, false, ImGuiFileDialogFlags.SelectOnly, callback );
        }

        public static void SaveFileDialog( string title, string filters, string defaultFileName, string defaultExtension, Action<bool, string> callback ) {
            SetDialog( "SaveFileDialog", title, filters, SavedPath, defaultFileName, defaultExtension, 1, false, ImGuiFileDialogFlags.None, callback );
        }

        private static void SetDialog(
            string id,
            string title,
            string filters,
            string path,
            string defaultFileName,
            string defaultExtension,
            int selectionCountMax,
            bool isModal,
            ImGuiFileDialogFlags flags,
            Action<bool, string> callback
        ) {
            if( ImagePreview ) flags |= ImGuiFileDialogFlags.LoadPreview;
            UnloadDialog();
            Callback = callback;
            Dialog = new FileDialog( PluginInterface, id, title, filters, path, defaultFileName, defaultExtension, selectionCountMax, isModal, Recent, flags );
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
            Dialog?.Dispose();
        }

        private static void AddRecent( string path ) {
            foreach( var recent in Recent ) {
                if( recent.Location == path ) return;
            }

            Recent.Add( new SideBarItem {
                Icon = ( char )FontAwesomeIcon.Folder,
                Location = path,
                Text = Path.GetFileName( path )
            } );

            while( Recent.Count > 10 ) {
                Recent.RemoveAt( 0 );
            }
        }
    }
}
