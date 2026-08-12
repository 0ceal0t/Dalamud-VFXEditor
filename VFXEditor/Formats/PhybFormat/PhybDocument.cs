using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.PhybFormat {
    public class PhybDocument : FileManagerBasicDocument<PhybFile> {
        public override string Id => "Phyb";
        public override string Extension => "phyb";

        public PhybDocument( PhybManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public PhybDocument( PhybManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override PhybFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, Source.Path, verify );
    }
}
