using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    // Document for formats that need no custom behavior
    public class FormatDocument<F> : FileManagerBasicDocument<F> where F : FileManagerFile, IReadableFile<F> {
        public override string Id => Manager.FormatName;
        public override string Extension => Manager.Extension;

        public FormatDocument( FileManagerBase manager, string writeLocation ) : base( manager, writeLocation ) { }

        public FormatDocument( FileManagerBase manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override F FileFromReader( BinaryReader reader, bool verify ) => F.Read( reader, Source?.Path, verify );
    }
}