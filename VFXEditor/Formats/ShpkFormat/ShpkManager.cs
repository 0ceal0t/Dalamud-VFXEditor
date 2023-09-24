using VfxEditor.FileManager;
using VfxEditor.Select.Shpk;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ShpkFormat {
    public unsafe class ShpkManager : FileManager<ShpkDocument, ShpkFile, WorkspaceMetaBasic> {
        public ShpkManager() : base( "Shpk Editor", "Shpk" ) {
            SourceSelect = new ShpkSelectDialog( "Shpk Select [LOADED]", this, true );
            ReplaceSelect = new ShpkSelectDialog( "Shpk Select [REPLACED]", this, false );
        }

        protected override ShpkDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override ShpkDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
