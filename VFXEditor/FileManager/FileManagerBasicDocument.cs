using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerBasicDocument<R> : FileManagerDocument<R, WorkspaceMetaBasic> where R : FileManagerFile {
        protected FileManagerBasicDocument( FileManagerBase manager, string writeLocation ) : base( manager, writeLocation ) { }

        protected FileManagerBasicDocument( FileManagerBase manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath, int windowIdx ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled,
            WindowIndex = windowIdx
        };
    }
}
