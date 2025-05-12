using System;
using VfxEditor.Data.Command;

namespace VfxEditor.FileBrowser {
    public static class FileBrowserManager {
        private static FileBrowserDialog Dialog;
        private static string SavedPath = ".";
        private static Action<bool, string> Callback;
        private static Action<bool, string[]> MultiSelectCallback;

        public static void Dispose() {
            Reset();
        }

        // Original method for single file selection
        public static void OpenFileDialog( string title, string filters, Action<bool, string> callback ) {
            SetDialog( "OpenFileDialog", title, filters, ".", "", false, ImGuiFileDialogFlags.SelectOnly, false, callback, null );
        }

        // New overload for multi-file selection
        public static void OpenFileDialogMultiple( string title, string filters, Action<bool, string[]> callback ) {
            var flags = ImGuiFileDialogFlags.SelectOnly | ImGuiFileDialogFlags.MultiSelect;
            SetDialog( "OpenFileDialog", title, filters, ".", "", false, flags, false, null, callback );
        }

        public static void OpenFileModal( string title, string filters, Action<bool, string> callback ) {
            SetDialog( "OpenFileDialog", title, filters, ".", "", true, ImGuiFileDialogFlags.SelectOnly, false, callback, null );
        }

        public static void SaveFileDialog( string title, string filters, string defaultFileName, string defaultExtension, Action<bool, string> callback ) {
            SetDialog( "SaveFileDialog", title, filters, defaultFileName, defaultExtension, false, ImGuiFileDialogFlags.ConfirmOverwrite, false, callback, null );
        }

        // These 2 aren't actually used, but save them just in case
        public static void OpenFolderDialog( string title, Action<bool, string> callback ) {
            SetDialog( "OpenFolderDialog", title, "", ".", "", false, ImGuiFileDialogFlags.SelectOnly, true, callback, null );
        }

        public static void SaveFolderDialog( string title, string defaultFolderName, Action<bool, string> callback ) {
            SetDialog( "SaveFolderDialog", title, "", defaultFolderName, "", false, ImGuiFileDialogFlags.None, true, callback, null );
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
            Action<bool, string> callback,
            Action<bool, string[]> multiSelectCallback
        ) {
            Reset();
            Callback = callback;
            MultiSelectCallback = multiSelectCallback;
            // Save CommandManager so we can use it for later
            Dialog = new FileBrowserDialog( id, title, modal, flags, folderDialog, filters, SavedPath, defaultFileName, defaultExtension, CommandManager.Current );
            Dialog.Show();
        }

        public static void Draw() {
            if( Dialog == null ) return;
            if( Dialog.Draw() ) {
                using var command = new CommandRaii( Dialog.Command );
                var isOk = Dialog.GetIsOk();
                var results = Dialog.GetResults();
                
                if (MultiSelectCallback != null) {
                    MultiSelectCallback(isOk, results);
                }
                else if (Callback != null) {
                    // For single-select, just pass the first result
                    Callback(isOk, results.Length > 0 ? results[0] : null);
                }

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
            MultiSelectCallback = null;
        }
    }
}
