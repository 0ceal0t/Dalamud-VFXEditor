using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.PapFormat {
    public class PapManagerGroup : FileManagerGroup<PapManager, PapDocument, PapFile, WorkspaceMetaBasic> {
        public PapManagerGroup() : base( "Pap Editor", "Pap" ) { }

        protected override PapManager GetNewManager() => new( this );
    }
}
