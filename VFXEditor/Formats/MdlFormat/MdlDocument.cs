using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MdlFormat {
    public class MdlDocument : FileManagerBasicDocument<MdlFile> {
        public override string Id => "Mdl";
        public override string Extension => "mdl";

        public MdlDocument( MdlManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public MdlDocument( MdlManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override MdlFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );
    }
}
