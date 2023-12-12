using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbFormat {
    public unsafe class SgbManager : FileManager<SgbDocument, SgbFile, WorkspaceMetaBasic> {
        public SgbManager() : base( "Sgb Editor", "Sgb" ) {
            SourceSelect = new SgbSelectDialog( "Sgb Select [LOADED]", this, true );
            ReplaceSelect = new SgbSelectDialog( "Sgb Select [REPLACED]", this, false );
        }

        protected override SgbDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override SgbDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
