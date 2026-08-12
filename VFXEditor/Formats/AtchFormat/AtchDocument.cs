using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AtchFormat {
    public class AtchDocument : FileManagerBasicDocument<AtchFile> {
        public override string Id => "Atch";
        public override string Extension => "atch";

        public AtchDocument( AtchManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public AtchDocument( AtchManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override AtchFile FileFromReader( BinaryReader reader, bool verify ) => new( reader );
    }
}
