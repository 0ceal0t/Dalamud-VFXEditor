using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGuiFileDialog {
    public class FileDialogManager {

        private static FileDialog Dialog;
        private static string SavedPath = ".";
        private static Action<bool, string> Callback;

        public static void OpenFolderDialog( string title, Action<bool, string> callback ) {
            SetDialog("OpenFolderDialog", title, "", SavedPath, ".", "", 1, false, ImGuiFileDialogFlags.SelectOnly, callback);
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

            Dispose();
            Callback = callback;
            Dialog = new FileDialog( id, title, filters, path, defaultFileName, defaultExtension, selectionCountMax, isModal, flags );
            Dialog.Show();
        }

        public static void Draw() {
            if( Dialog == null ) return;
            if(Dialog.Draw()) {
                Callback( Dialog.GetIsOk(), Dialog.GetResult() );
                SavedPath = Dialog.GetCurrentPath();
                Dispose();
            }
        }

        public static void Dispose() {
            Dialog?.Hide();
            Dialog = null;
            Callback = null;
        }
    }
}
