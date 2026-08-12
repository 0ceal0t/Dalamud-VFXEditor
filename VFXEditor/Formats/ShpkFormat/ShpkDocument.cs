using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ShpkFormat {
    public class ShpkDocument : FileManagerBasicDocument<ShpkFile> {
        public override string Id => "Shpk";
        public override string Extension => "shpk";

        public ShpkDocument( ShpkManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public ShpkDocument( ShpkManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override ShpkFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );
    }
}
