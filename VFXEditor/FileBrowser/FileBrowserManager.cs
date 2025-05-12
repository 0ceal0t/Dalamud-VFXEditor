using System;
using VfxEditor.Data.Command;

namespace VfxEditor.FileBrowser {
    public static class FileBrowserManager {
        private static FileBrowserDialog Dialog;
        private static string SavedPath = ".";
        private static Action<bool, string> Callback;

        public static void Dispose() {
            Reset();
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

        // These 2 aren't actually used, but save them just in case
        public static void OpenFolderDialog( string title, Action<bool, string> callback ) {
            SetDialog( "OpenFolderDialog", title, "", ".", "", false, ImGuiFileDialogFlags.SelectOnly, true, callback );
        }
        public static void SaveFolderDialog( string title, string defaultFolderName, Action<bool, string> callback ) {
            SetDialog( "SaveFolderDialog", title, "", defaultFolderName, "", false, ImGuiFileDialogFlags.None, true, callback );
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
            Reset();
            Callback = callback;
            // Save CommandManager so we can use it for later
            Dialog = new FileBrowserDialog( id, title, modal, flags, folderDialog, filters, SavedPath, defaultFileName, defaultExtension, CommandManager.Current );
            Dialog.Show();
        }

        public static void Draw() {
            if( Dialog == null ) return;
            if( Dialog.Draw() ) {
                using var command = new CommandRaii( Dialog.Command );
                Callback( Dialog.GetIsOk(), Dialog.GetResult() );

                SavedPath = Dialog.GetCurrentPath();
                Plugin.Configuration.AddFileBrowserRecent( SavedPath );
                Reset();
            }
        }

        private static void Reset() {
            Dialog?.Dispose();
            Dialog?.Hide();
            Dialog = null;
            Callback = null;
        }
    }
}
