using System;

namespace ImGuiFileDialog {
    public class FileDialogManager {

        private FileDialog Dialog;
        private string SavedPath = ".";
        private Action<bool, string> Callback;

        public void OpenFolderDialog( string title, Action<bool, string> callback ) {
            SetDialog("OpenFolderDialog", title, "", SavedPath, ".", "", 1, false, ImGuiFileDialogFlags.SelectOnly, callback);
        }

        public void SaveFolderDialog( string title, string defaultFolderName, Action<bool, string> callback ) {
            SetDialog( "SaveFolderDialog", title, "", SavedPath, defaultFolderName, "", 1, false, ImGuiFileDialogFlags.None, callback );
        }

        public void OpenFileDialog( string title, string filters, Action<bool, string> callback ) {
            SetDialog( "OpenFileDialog", title, filters, SavedPath, ".", "", 1, false, ImGuiFileDialogFlags.SelectOnly, callback );
        }

        public void SaveFileDialog( string title, string filters, string defaultFileName, string defaultExtension, Action<bool, string> callback ) {
            SetDialog( "SaveFileDialog", title, filters, SavedPath, defaultFileName, defaultExtension, 1, false, ImGuiFileDialogFlags.None, callback );
        }

        private void SetDialog(
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
