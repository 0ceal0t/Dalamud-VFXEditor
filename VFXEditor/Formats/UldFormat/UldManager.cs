using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.UldFormat {
    public unsafe class UldManager : FileManager<UldDocument, UldFile, WorkspaceMetaRenamed> {
        public UldManager( UldManagerGroup group ) : base( group ) {
            SourceSelect = new UldSelectDialog( "Uld Select [LOADED]", this, true );
            ReplaceSelect = new UldSelectDialog( "Uld Select [REPLACED]", this, false );
        }

        protected override UldDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override UldDocument GetWorkspaceDocument( WorkspaceMetaRenamed data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
