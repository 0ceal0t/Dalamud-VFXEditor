using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.PhybFormat {
    public class PhybManager : FileManager<PhybDocument, PhybFile, WorkspaceMetaBasic> {
        public PhybManager( PhybManagerGroup group ) : base( group ) {
            SourceSelect = new PhybSelectDialog( "Phyb Select [LOADED]", this, true );
            ReplaceSelect = new PhybSelectDialog( "Phyb Select [REPLACED]", this, false );
        }

        protected override PhybDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override PhybDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}