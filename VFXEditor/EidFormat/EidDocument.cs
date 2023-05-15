using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public class EidDocument : FileManagerDocument<EidFile, WorkspaceMetaBasic> {
        public EidDocument( EidManager manager, string writeLocation ) : base( manager, writeLocation, "Eid", "eid" ) { }

        public EidDocument( EidManager manager, string writeLocation, string localPath, string name, SelectResult source, SelectResult replace ) :
            base( manager, writeLocation, localPath, name, source, replace, "Eid", "eid" ) { }

        protected override EidFile FileFromReader( BinaryReader reader ) => new( reader );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };
    }
}
