using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat {
    public partial class SklbDocument : FileManagerDocument<SklbFile, WorkspaceMetaBasic> {
        private string HkxTemp => WriteLocation.Replace( ".sklb", "_temp.hkx" );

        public SklbDocument( SklbManager manager, string writeLocation ) : base( manager, writeLocation, "Sklb", "sklb" ) { }

        public SklbDocument( SklbManager manager, string writeLocation, string localPath, string name, SelectResult source, SelectResult replace ) :
            base( manager, writeLocation, localPath, name, source, replace, "Sklb", "sklb" ) { }

        protected override SklbFile FileFromReader( BinaryReader reader ) => new( reader, HkxTemp );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };

        public override void Dispose() {
            base.Dispose();
            File.Delete( HkxTemp );
        }
    }
}
