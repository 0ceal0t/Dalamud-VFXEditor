using VfxEditor.FileManager;
using VfxEditor.Select.Phyb;
using VfxEditor.Utils;

namespace VfxEditor.PhybFormat {
    public class PhybManager : FileManager<PhybDocument, PhybFile, WorkspaceMetaBasic> {
        public PhybManager() : base( "Phyb Editor", "Phyb" ) {
            SourceSelect = new PhybSelectDialog( "Phyb Select [LOADED]", this, true );
            ReplaceSelect = new PhybSelectDialog( "Phyb Select [REPLACED]", this, false );
        }

        protected override PhybDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override PhybDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}