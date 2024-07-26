using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdManager : FileManager<PbdDocument, PbdFile, WorkspaceMetaBasic> {
        public PbdManager() : base( "Pbd Editor", "Pbd" ) {
            SourceSelect = new PbdSelectDialog( "Pbd Select [LOADED]", this, true );
            ReplaceSelect = new PbdSelectDialog( "Pbd Select [REPLACED]", this, false );
        }

        protected override PbdDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override PbdDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
