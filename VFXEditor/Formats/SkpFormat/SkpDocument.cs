using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SkpFormat {
    public class SkpDocument : FileManagerBasicDocument<SkpFile> {
        public override string Id => "Skp";
        public override string Extension => "skp";

        public SkpDocument( SkpManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public SkpDocument( SkpManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override SkpFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );
    }
}
