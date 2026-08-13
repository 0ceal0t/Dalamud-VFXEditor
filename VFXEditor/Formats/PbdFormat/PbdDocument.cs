using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdDocument : FileManagerBasicDocument<PbdFile> {
        public override string Id => "Pbd";
        public override string Extension => "pbd";

        public PbdDocument( PbdManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public PbdDocument( PbdManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override PbdFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );
    }
}
