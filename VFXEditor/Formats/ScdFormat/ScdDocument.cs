using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdDocument : FileManagerBasicDocument<ScdFile> {
        public override string Id => "Scd";
        public override string Extension => "scd";

        public ScdDocument( ScdManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public ScdDocument( ScdManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override ScdFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );
    }
}
