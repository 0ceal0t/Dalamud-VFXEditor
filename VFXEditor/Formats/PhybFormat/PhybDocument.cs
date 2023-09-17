using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.PhybFormat {
    public class PhybDocument : FileManagerDocument<PhybFile, WorkspaceMetaBasic> {
        public override string Id => "Phyb";
        public override string Extension => "phyb";

        public PhybDocument( PhybManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public PhybDocument( PhybManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override PhybFile FileFromReader( BinaryReader reader ) => new( reader, Source.Path, true );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
