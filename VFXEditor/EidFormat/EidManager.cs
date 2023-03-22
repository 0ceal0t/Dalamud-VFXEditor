using VfxEditor.FileManager;
using VfxEditor.Select.Eid;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public class EidManager : FileManagerWindow<EidDocument, EidFile, WorkspaceMetaBasic> {
        public EidManager() : base( "Eid Editor", "Eid", "eid", "Eid", "Eid" ) {
            SourceSelect = new EidSelectDialog( "Eid Select [LOADED]", this, true );
            ReplaceSelect = new EidSelectDialog( "Eid Select [REPLACED]", this, false );
        }

        protected override EidDocument GetNewDocument() => new( this, LocalPath );

        protected override EidDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) =>
            new( this, LocalPath, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data.Source, data.Replace );
    }
}
