using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat {
    public class KdbDocument : FileManagerDocument<KdbFile, WorkspaceMetaBasic> {
        public override string Id => "Kdb";
        public override string Extension => "kdb";

        public KdbDocument( KdbManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public KdbDocument( KdbManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override KdbFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
