using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDialog {
    [Flags]
    public enum ImGuiFileDialogFlags {
        None = 0,
        ConfirmOverwrite = 1,
        DontShowHiddenFiles = 2,
        DisableCreateDirectoryButton = 3,
        HideColumnType = 4,
        HideColumnSize = 5,
        HideColumnDate = 6
    }
}
