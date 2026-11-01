using VfxEditor.FileManager;
using VfxEditor.Formats.EidFormat;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public class EidManager : FileManager<EidDocument, EidFile, WorkspaceMetaBasic> {
        public EidManager( EidManagerGroup group ) : base( group ) {
            SourceSelect = new EidSelectDialog( "Eid Select [LOADED]", this, true );
            ReplaceSelect = new EidSelectDialog( "Eid Select [REPLACED]", this, false );
        }

        protected override EidDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override EidDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}