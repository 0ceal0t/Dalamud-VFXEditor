using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ShpkFormat {
    public class ShpkDocument : FileManagerDocument<ShpkFile, WorkspaceMetaBasic> {
        public override string Id => "Shpk";
        public override string Extension => "shpk";

        public ShpkDocument( ShpkManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public ShpkDocument( ShpkManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override ShpkFile FileFromReader( BinaryReader reader ) => new( reader, true );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
