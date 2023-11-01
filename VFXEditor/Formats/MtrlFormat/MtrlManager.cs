using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat {
    public unsafe class MtrlManager : FileManager<MtrlDocument, MtrlFile, WorkspaceMetaBasic> {
        public MtrlManager() : base( "Mtrl Editor", "Mtrl" ) {
            SourceSelect = new MtrlSelectDialog( "Mtrl Select [LOADED]", this, true );
            ReplaceSelect = new MtrlSelectDialog( "Mtrl Select [REPLACED]", this, false );
        }

        protected override MtrlDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override MtrlDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
