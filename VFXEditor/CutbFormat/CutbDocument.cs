using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.CutbFormat {
    public class CutbDocument : FileManagerDocument<CutbFile, WorkspaceMetaBasic> {
        private string HkxTemp => WriteLocation.Replace( ".cutb", "_temp.hkx" );

        public CutbDocument( CutbManager manager, string writeLocation ) : base( manager, writeLocation, "Cutb", "cutb" ) { }

        public CutbDocument( CutbManager manager, string writeLocation, string localPath, string name, SelectResult source, SelectResult replace ) :
            base( manager, writeLocation, localPath, name, source, replace, "Cutb", "cutb" ) { }

        protected override CutbFile FileFromReader( BinaryReader reader ) => new( reader, HkxTemp );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };
    }
}
