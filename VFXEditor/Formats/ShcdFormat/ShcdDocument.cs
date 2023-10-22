using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ShcdFormat {
    public class ShcdDocument : FileManagerDocument<ShcdFile, WorkspaceMetaBasic> {
        public override string Id => "Shcd";
        public override string Extension => "shcd";

        public ShcdDocument( ShcdManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public ShcdDocument( ShcdManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override ShcdFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
