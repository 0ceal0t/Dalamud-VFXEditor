using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MdlFormat {
    public unsafe class MdlManager : FileManager<MdlDocument, MdlFile, WorkspaceMetaBasic> {
        public MdlManager() : base( "Mdl Editor", "Mdl" ) {
            SourceSelect = new MdlSelectDialog( "Mdl Select [LOADED]", this, true );
            ReplaceSelect = new MdlSelectDialog( "Mdl Select [REPLACED]", this, false );
        }

        protected override MdlDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override MdlDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
