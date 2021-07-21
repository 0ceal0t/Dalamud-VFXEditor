# ImGuiFileDialog

Ported mostly verbatim from the original [ImGuiDialog](https://github.com/aiekick/ImGuiFileDialog) library, with some usability changes

## How to Use

You can either create a `FileDialog` directly, or use a `FileDialogManager` to handle things for you

### `FileDialogManager`
```
using ImGuiFileDialog;

FileDialogManager fd = new FileDialogManager();
PluginInterface.UiBuilder.OnBuildUi += fd.Draw;

// on dispose...
PluginInterface.UiBuilder.OnBuildUi -= fd.Draw;
fd.Dispose();
```

Then, you can create dialogs like this. Note that only one dialog can be active at a time.
```
fd.SelectFolderDialog("ID_1", "Selecte a Folder", (bool ok, string result) => {

    // ...

});

fd.OpenFileDialog("ID_2", "Selecte a File to Open", ".json,.*", (bool ok, string result) => {

    // NOTE: see the section on filters for information on how to format them
    // ...

});

fd.SaveFileDialog("ID_3", "Selecte a Location to Save", ".json,.*", "default_name", "json", (bool ok, string result) => {

    // NOTE: see the section on filters for information on how to format them
    // ...

});
```

### `FileDialog`

#### Constructor

- `id`: A unique id used for the main ImGui window
- `title`: The title at the top of the window
- `filters`: See the "Filters" section for more information. Leave this empty to switch to directory mode.
- `path`: The starting path for the dialog
- `defaultFileName`: The default file name. For directory mode, leave as `"."`
- `defaultExtension`: The default extension, such as `json`. For directory mode, leave as `""`
- `selectionCountMax`: How many files or directories can be selected at once. Set to `0` for an infinite number
- `isModal`: Whether the dialog will be a window or popup modal
- `flags`: See the "Flags" section

#### Methods

- `void Show()`
- `void Hide()`
- `bool Draw()`: Returns `true` when the dialog has a result
- `bool GetIsOk()`: Returns `true` if the dialog successfully selected a file or directory
- `string GetResult()`: Gets the result of the dialog. If multiple were selected, the results are separated by commas
- `void SetPath(string path)`
- `string GetCurrentPath()`

### Flags

- `ConfirmOverwrite`: When selecting an existing file, should the selection be confirmed
- `SelectOnly`: The results must be from existing files or directories
- `DontShowHiddenFiles`
- `DisableCreateDirectoryButton`
- `HideColumnType`
- `HideColumnSize`
- `HideColumnDate`

### Filters

If you wish the create a dialog in directory mode, set the filters to `""`. Otherwise, filters can follow formats such as these:
```
.*
.*,.cpp,.h,.hpp
.atex
Image files{.png,.gif,.jpg,.jpeg}
.*,Source files{.cpp,.h,.hpp}
```