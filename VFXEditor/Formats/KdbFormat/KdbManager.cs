using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat {
    public class KdbManager : FileManager<KdbDocument, KdbFile, WorkspaceMetaBasic> {
        public KdbManager() : base( "Kdb Editor", "Kdb" ) {
            SourceSelect = new KdbSelectDialog( "Kdb Select [LOADED]", this, true );
            ReplaceSelect = new KdbSelectDialog( "Kdb Select [REPLACED]", this, false );
        }

        protected override KdbDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override KdbDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
