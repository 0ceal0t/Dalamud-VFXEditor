using VfxEditor.FileManager;
using VfxEditor.Select.Pap;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public class PapManager : FileManagerWindow<PapDocument, PapFile, WorkspaceMetaBasic> {
        public PapManager() : base( "Pap Editor", "Pap" ) {
            SourceSelect = new PapSelectDialog( "Pap Select [LOADED]", this, true );
            ReplaceSelect = new PapSelectDialog( "Pap Select [REPLACED]", this, false );
        }

        protected override PapDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override PapDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) =>
            new( this, NewWriteLocation, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data.Name, data.Source, data.Replace );
    }
}
