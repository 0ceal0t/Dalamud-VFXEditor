using VfxEditor.FileManager;
using VfxEditor.Select.Sklb;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat {
    public class SklbManager : FileManagerWindow<SklbDocument, SklbFile, WorkspaceMetaBasic> {
        public SklbManager() : base( "Sklb Editor", "Sklb" ) {
            SourceSelect = new SklbSelectDialog( "Sklb Select [LOADED]", this, true );
            ReplaceSelect = new SklbSelectDialog( "Sklb Select [REPLACED]", this, false );
        }

        protected override SklbDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override SklbDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) =>
            new( this, NewWriteLocation, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data.Name, data.Source, data.Replace );
    }
}
