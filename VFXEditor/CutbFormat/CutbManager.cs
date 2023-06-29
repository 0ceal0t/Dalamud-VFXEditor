using VfxEditor.FileManager;
using VfxEditor.Select.Cutb;
using VfxEditor.Utils;

namespace VfxEditor.CutbFormat {
    public unsafe class CutbManager : FileManagerWindow<CutbDocument, CutbFile, WorkspaceMetaBasic> {
        public CutbManager() : base( "Cutb Editor", "Cutb" ) {
            SourceSelect = new CutbSelectDialog( "Cutb Select [LOADED]", this, true );
            ReplaceSelect = new CutbSelectDialog( "Cutb Select [REPLACED]", this, false );
        }

        protected override CutbDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override CutbDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) =>
            new( this, NewWriteLocation, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data.Name, data.Source, data.Replace );
    }
}
