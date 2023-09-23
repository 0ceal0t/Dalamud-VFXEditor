using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SkpFormat {
    public class SkpDocument : FileManagerDocument<SkpFile, WorkspaceMetaBasic> {
        public override string Id => "Skp";
        public override string Extension => "skp";

        public SkpDocument( SkpManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public SkpDocument( SkpManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
        }

        protected override SkpFile FileFromReader( BinaryReader reader ) => new( reader, true );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
