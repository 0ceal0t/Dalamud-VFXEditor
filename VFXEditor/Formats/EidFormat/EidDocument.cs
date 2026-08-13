using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public class EidDocument : FileManagerBasicDocument<EidFile> {
        public override string Id => "Eid";
        public override string Extension => "eid";

        public EidDocument( EidManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public EidDocument( EidManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override EidFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, Source.Path, verify );
    }
}
