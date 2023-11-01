using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ShcdFormat {
    public unsafe class ShcdManager : FileManager<ShcdDocument, ShcdFile, WorkspaceMetaBasic> {
        public ShcdManager() : base( "Shcd Editor", "Shcd" ) {
            SourceSelect = new ShcdSelectDialog( "Shcd Select [LOADED]", this, true );
            ReplaceSelect = new ShcdSelectDialog( "Shcd Select [REPLACED]", this, false );
        }

        protected override ShcdDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override ShcdDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
