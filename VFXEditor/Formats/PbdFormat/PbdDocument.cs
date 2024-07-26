using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdDocument : FileManagerDocument<PbdFile, WorkspaceMetaBasic> {
        public override string Id => "Pbd";
        public override string Extension => "pbd";

        public PbdDocument( PbdManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public PbdDocument( PbdManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override PbdFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
