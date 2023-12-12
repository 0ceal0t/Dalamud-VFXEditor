using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbFormat {
    public class SgbDocument : FileManagerDocument<SgbFile, WorkspaceMetaBasic> {
        public override string Id => "Sgb";
        public override string Extension => "sgb";

        public SgbDocument( SgbManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public SgbDocument( SgbManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override SgbFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
