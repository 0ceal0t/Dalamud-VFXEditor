using VfxEditor.FileManager.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerRenamedDocument<R> : FileManagerDocument<R, WorkspaceMetaRenamed> where R : FileManagerFile, IRenamableFile {
        protected FileManagerRenamedDocument( FileManagerBase manager, string writeLocation ) : base( manager, writeLocation ) { }

        protected FileManagerRenamedDocument( FileManagerBase manager, string writeLocation, string localPath, WorkspaceMetaRenamed data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
            File?.ReadRenamingMap( data.Renaming );
        }

        public override WorkspaceMetaRenamed GetWorkspaceMeta( string newPath, int windowIdx ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Renaming = File.GetRenamingMap(),
            Disabled = Disabled,
            WindowIndex = windowIdx
        };
    }
}
