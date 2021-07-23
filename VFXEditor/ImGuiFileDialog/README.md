# ImGuiFileDialog

Ported mostly verbatim from the original [ImGuiDialog](https://github.com/aiekick/ImGuiFileDialog) library, with some usability changes

## How to Use

You can either create a `FileDialog` directly, or use a `FileDialogManager` to handle things for you

## Using the Manager
```cs
var dialogManager = new FileDialogManager();

PluginInterface.UiBuilder.OnBuildUi += dialogManager.Draw;

// on dispose...
PluginInterface.UiBuilder.OnBuildUi -= dialogManager.Draw;
```

Then, you can create dialogs like this. Note that only one dialog can be active at a time.
```cs
dialogManager.OpenFolderDialog("Select a Folder", (bool ok, string result) => {
    // ...
});

dialogManager.SaveFolderDialog("Select a Folder to Save", "default_folder_name", (bool ok, string result) => {
    // ...
});

dialogManager.OpenFileDialog("Select a File to Open", ".json,.*", (bool ok, string result) => {
    // NOTE: see the section on filters for information on how to format them
});

dialogManager.SaveFileDialog("Select a Location to Save", ".json,.*", "default_file_name", "json", (bool ok, string result) => {
    // NOTE: see the section on filters for information on how to format them
});
```

## Creating a `FileDialog`

```cs
/*
 * id: A unique id used for the main ImGui window
 * title: The title at the top of the window
 * filters: See the "Filters" section for more information. Leave this empty to switch to directory mode
 * path: The starting path for the dialog
 * defaultFileName: The default file name. For directory mode, leave as "."
 * defaultExtension: The default extension, such as `json`. For directory mode, leave as ""
 * selectionCountMax: How many files or directories can be selected at once. Set to `0` for an infinite number
 * isModal: Whether the dialog will be a window or popup modal
 * flags:  See the "Flags" section
 */

FileDialog(string id, string title, string filters, string path, string defaultFileName, string defaultExtension, int selectionCountMax, bool isModal, ImGuiFileDialogFlags flags)

// ==== METHODS =====

void Show();

void Hide();

// Returns "true' when the dialog has a result
bool Draw();

// Returns "true" if the dialog successfully selected a file or directory
bool GetIsOk();

string GetCurrentPath();
```

## Flags

```cs
ImGuiFileDialogFlags.None;
ImGuiFileDialogFlags.ConfirmOverwrite; // The results must be from existing files or directories
ImGuiFileDialogFlags.SelectOnly; // The results must be from existing files or directories
ImGuiFileDialogFlags.DontShowHiddenFiles;
ImGuiFileDialogFlags.DisableCreateDirectoryButton;
ImGuiFileDialogFlags.HideColumnType;
ImGuiFileDialogFlags.HideColumnSize;
ImGuiFileDialogFlags.HideColumnDate;
```

## Filters

If you wish the create a dialog in directory mode, set the filters to `""`. Otherwise, filters can follow formats such as these:

```
.*
.*,.cpp,.h,.hpp
.atex
Image files{.png,.gif,.jpg,.jpeg}
.*,Source files{.cpp,.h,.hpp}
```