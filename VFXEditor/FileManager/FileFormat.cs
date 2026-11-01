using System;
using System.Collections.Generic;
using VfxEditor.Select;
using VfxEditor.Select.Base;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    // Editors in the same category are grouped together in the menu bar
    public enum FormatCategory {
        Vfx,
        Animation,
        Sound,
        Ui,
        Skeleton,
        Model,
    }

    // Everything needed to create an editor for a file format, see Formats/FileFormats.cs
    public class FileFormat<D, F, S> where D : FileManagerDocument<F, S> where F : FileManagerFile {
        // Used for the window title, the workspace key, the /vfxedit argument, and (lowercase) the file extension
        public required string Name { get; init; }
        public required FormatCategory Category { get; init; }
        public required Func<FileManagerBase, string, D> NewDocument { get; init; }
        public required Func<FileManagerBase, string, string, S, D> WorkspaceDocument { get; init; }

        // Tabs listing game files in the select dialog
        public Func<SelectDialog, IEnumerable<SelectTab>> SelectTabs { get; init; } = _ => [];
        // Replaces the default select dialog, for dialogs with extra features (such as a play button)
        public Func<string, FileManagerBase, bool, SelectDialog> CustomSelectDialog { get; init; }

        public string Title => $"{Name} Editor";
        public string Extension => Name.ToLower();

        public SelectDialog CreateSelectDialog( FileManagerBase manager, bool isSourceDialog ) {
            var title = $"{Name} Select [{( isSourceDialog ? "LOADED" : "REPLACED" )}]";
            return CustomSelectDialog?.Invoke( title, manager, isSourceDialog ) ?? new FormatSelectDialog( title, Extension, manager, isSourceDialog, SelectTabs );
        }

        public FormatManagerGroup<D, F, S> CreateGroup() => new( this );
    }

    public static class FileFormat {
        // A format whose documents need no custom behavior
        public static FileFormat<FormatDocument<F>, F, WorkspaceMetaBasic> Basic<F>( string name, FormatCategory category, Func<SelectDialog, IEnumerable<SelectTab>> selectTabs )
            where F : FileManagerFile, IReadableFile<F> => new() {
                Name = name,
                Category = category,
                SelectTabs = selectTabs,
                NewDocument = ( manager, writeLocation ) => new( manager, writeLocation ),
                WorkspaceDocument = ( manager, writeLocation, localPath, data ) => new( manager, writeLocation, localPath, data ),
            };
    }
}