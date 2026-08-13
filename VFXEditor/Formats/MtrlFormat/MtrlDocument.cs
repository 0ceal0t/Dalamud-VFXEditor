using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat {
    public class MtrlDocument : FileManagerBasicDocument<MtrlFile> {
        public override string Id => "Mtrl";
        public override string Extension => "mtrl";

        public MtrlDocument( MtrlManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public MtrlDocument( MtrlManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override MtrlFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );
    }
}
