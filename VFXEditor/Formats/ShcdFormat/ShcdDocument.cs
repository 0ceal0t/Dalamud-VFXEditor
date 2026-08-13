using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ShcdFormat {
    public class ShcdDocument : FileManagerBasicDocument<ShcdFile> {
        public override string Id => "Shcd";
        public override string Extension => "shcd";

        public ShcdDocument( ShcdManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public ShcdDocument( ShcdManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override ShcdFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );
    }
}
