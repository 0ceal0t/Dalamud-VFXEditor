using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public class PapManager : FileManager<PapDocument, PapFile, WorkspaceMetaBasic> {
        public PapManager() : base( "Pap Editor", "Pap" ) {
            SourceSelect = new PapSelectDialog( "Pap Select [LOADED]", this, true );
            ReplaceSelect = new PapSelectDialog( "Pap Select [REPLACED]", this, false );
        }

        protected override PapDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override PapDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
