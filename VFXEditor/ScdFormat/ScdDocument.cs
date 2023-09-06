using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdDocument : FileManagerDocument<ScdFile, WorkspaceMetaBasic> {
        public ScdDocument( ScdManager manager, string writeLocation ) : base( manager, writeLocation, "Scd", "scd" ) { }

        public ScdDocument( ScdManager manager, string writeLocation, string localPath, string name, SelectResult source, SelectResult replace, bool disabled ) :
                base( manager, writeLocation, localPath, name, source, replace, disabled, "Scd", "scd" ) { }

        protected override ScdFile FileFromReader( BinaryReader reader ) => new( reader );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Disabled = Disabled
        };
    }
}
