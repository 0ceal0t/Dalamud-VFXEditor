using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AtchFormat {
    public class AtchDocument : FileManagerDocument<AtchFile, WorkspaceMetaBasic> {
        public override string Id => "Atch";
        public override string Extension => "atch";

        public AtchDocument( AtchManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public AtchDocument( AtchManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override AtchFile FileFromReader( BinaryReader reader ) => new( reader );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
