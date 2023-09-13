using VfxEditor.FileManager;
using VfxEditor.Select.Atch;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AtchFormat {
    public unsafe class AtchManager : FileManager<AtchDocument, AtchFile, WorkspaceMetaBasic> {
        public AtchManager() : base( "Atch Editor", "Atch" ) {
            SourceSelect = new AtchSelectDialog( "Atch Select [LOADED]", this, true );
            ReplaceSelect = new AtchSelectDialog( "Atch Select [REPLACED]", this, false );
        }

        protected override AtchDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override AtchDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
