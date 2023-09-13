using VfxEditor.Data;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Ui;

namespace VfxEditor.FileManager {
    public abstract class FileManagerBase : GenericDialog, IFileManagerSelect {
        public readonly string Id;

        public abstract string NewWriteLocation { get; }

        protected FileManagerBase( string name, string id ) : base( name, true, 800, 1000 ) {
            Id = id;
        }

        public abstract ManagerConfiguration GetConfig();

        public abstract CopyManager GetCopyManager();

        public abstract CommandManager GetCommandManager();

        public abstract void SetSource( SelectResult result );

        public abstract void ShowSource();

        public abstract void SetReplace( SelectResult result );

        public abstract void ShowReplace();

        public abstract void Unsaved();

        public string GetId() => Id;
    }
}
