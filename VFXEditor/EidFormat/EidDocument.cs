using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public class EidDocument : FileManagerDocument<EidFile, WorkspaceMetaBasic> {
        public override string Id => "Eid";
        public override string Extension => "eid";

        public EidDocument( EidManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public EidDocument( EidManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override EidFile FileFromReader( BinaryReader reader ) => new( reader );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
