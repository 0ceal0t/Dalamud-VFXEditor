using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerHavokDocument<R> : FileManagerBasicDocument<R> where R : FileManagerFile {
        protected string HkxTemp => WriteLocation.Replace( $".{Extension}", "_temp.hkx" );

        protected FileManagerHavokDocument( FileManagerBase manager, string writeLocation ) : base( manager, writeLocation ) { }

        protected FileManagerHavokDocument( FileManagerBase manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        public override void Dispose() {
            base.Dispose();
            System.IO.File.Delete( HkxTemp );
        }
    }
}
