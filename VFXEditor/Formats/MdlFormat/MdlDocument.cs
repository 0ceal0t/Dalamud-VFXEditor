using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MdlFormat {
    public class MdlDocument : FileManagerDocument<MdlFile, WorkspaceMetaBasic> {
        public override string Id => "Mdl";
        public override string Extension => "mdl";

        public MdlDocument( MdlManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public MdlDocument( MdlManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override MdlFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
