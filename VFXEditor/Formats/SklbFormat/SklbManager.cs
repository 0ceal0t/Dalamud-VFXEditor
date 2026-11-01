using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SklbFormat {
    public class SklbManager : FileManager<SklbDocument, SklbFile, WorkspaceMetaBasic> {
        public SklbManager( SklbManagerGroup group ) : base( group ) {
            SourceSelect = new SklbSelectDialog( "Sklb Select [LOADED]", this, true );
            ReplaceSelect = new SklbSelectDialog( "Sklb Select [REPLACED]", this, false );
        }

        protected override SklbDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override SklbDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
