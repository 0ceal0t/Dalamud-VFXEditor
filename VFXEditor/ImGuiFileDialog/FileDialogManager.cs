using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGuiFileDialog {
    public class FileDialogManager {
        private FileDialog Dialog; // only show one at a time
        private string SavedPath = ".";
        private Action<bool, string> Callback;

        public FileDialogManager() { }

        public void SelectFolderDialog( string id, string title, Action<bool, string> callback ) {
            SelectFolderDialog( id, title, "", 1, false, ImGuiFileDialogFlags.None, callback );
        }
        public void SelectFolderDialog( string id, string title, string startingPath, int selectionCount, bool isModal, ImGuiFileDialogFlags flags, Action<bool, string> callback ) {
            flags = flags | ImGuiFileDialogFlags.SelectOnly;
            SetDialog( id, title, "", startingPath, ".", "", selectionCount, isModal, flags, callback );
        }

        public void SaveFileDialog( string id, string title, string filters, string defaultFileName, string defaultExtension, Action<bool, string> callback ) {
            SaveFileDialog( id, title, filters, "", defaultFileName, defaultExtension, false, ImGuiFileDialogFlags.None, callback );
        }
        public void SaveFileDialog( string id, string title, string filters, string startingPath, string defaultFileName, string defaultExtension, bool isModal, ImGuiFileDialogFlags flags, Action<bool, string> callback ) {
            flags = flags | ImGuiFileDialogFlags.ConfirmOverwrite;
            SetDialog( id, title, filters, startingPath, defaultFileName, defaultExtension, 1, isModal, flags, callback );
        }

        public void OpenFileDialog( string id, string title, string filters, Action<bool, string> callback ) {
            OpenFileDialog( id, title, filters, "", 1, false, ImGuiFileDialogFlags.None, callback );
        }
        public void OpenFileDialog(string id, string title, string filters, string startingPath, int selectionCount, bool isModal, ImGuiFileDialogFlags flags, Action<bool, string> callback) {
            flags = flags | ImGuiFileDialogFlags.SelectOnly;
            SetDialog( id, title, filters, startingPath, ".", "", selectionCount, isModal, flags, callback );
        }

        private void SetDialog(
            string id, string title, string filters, string path, string defaultFileName,
            string defaultExtension, int selectionCountMax, bool isModal, ImGuiFileDialogFlags flags, Action<bool, string> callback) {

            if(string.IsNullOrEmpty(path)) path = SavedPath;

            Dispose();
            Callback = callback;
            Dialog = new FileDialog( id, title, filters, path, defaultFileName, defaultExtension, selectionCountMax, isModal, flags );
            Dialog.Show();
        }

        public void Draw() {
            if( Dialog == null ) return;
            if(Dialog.Draw()) {
                Callback( Dialog.GetIsOk(), Dialog.GetResult() );
                SavedPath = Dialog.GetCurrentPath();
                Dispose();
            }
        }

        public void Dispose() {
            Dialog?.Hide();
            Dialog = null;
            Callback = null;
        }
    }
}
