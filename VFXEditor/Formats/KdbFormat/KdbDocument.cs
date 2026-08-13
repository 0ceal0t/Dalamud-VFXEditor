using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat {
    public class KdbDocument : FileManagerBasicDocument<KdbFile> {
        public override string Id => "Kdb";
        public override string Extension => "kdb";

        public KdbDocument( KdbManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public KdbDocument( KdbManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override KdbFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, Source.Path, verify );
    }
}
