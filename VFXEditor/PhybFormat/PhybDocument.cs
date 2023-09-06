using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.PhybFormat {
    public class PhybDocument : FileManagerDocument<PhybFile, WorkspaceMetaBasic> {
        public PhybDocument( PhybManager manager, string writeLocation ) : base( manager, writeLocation, "Phyb", "phyb" ) { }

        public PhybDocument( PhybManager manager, string writeLocation, string localPath, string name, SelectResult source, SelectResult replace, bool disabled ) :
                base( manager, writeLocation, localPath, name, source, replace, disabled, "Phyb", "phyb" ) { }

        protected override PhybFile FileFromReader( BinaryReader reader ) => new( reader, Source.Path );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
