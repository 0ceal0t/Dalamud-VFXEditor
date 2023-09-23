using VfxEditor.FileManager;
using VfxEditor.Select.Skp;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SkpFormat {
    public unsafe class SkpManager : FileManager<SkpDocument, SkpFile, WorkspaceMetaBasic> {
        public SkpManager() : base( "Skp Editor", "Skp" ) {
            SourceSelect = new SkpSelectDialog( "Skp Select [LOADED]", this, true );
            ReplaceSelect = new SkpSelectDialog( "Skp Select [REPLACED]", this, false );
        }

        protected override SkpDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override SkpDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
