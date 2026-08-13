using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.UldFormat {
    public class UldDocument : FileManagerRenamedDocument<UldFile> {
        public override string Id => "Uld";
        public override string Extension => "uld";

        public UldDocument( UldManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public UldDocument( UldManager manager, string writeLocation, string localPath, WorkspaceMetaRenamed data ) : base( manager, writeLocation, localPath, data ) { }

        protected override UldFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );
    }
}
