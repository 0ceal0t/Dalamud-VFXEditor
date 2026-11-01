using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.EidFormat {
    public class EidManagerGroup : FileManagerGroup<EidManager, EidDocument, EidFile, WorkspaceMetaBasic> {
        public EidManagerGroup() : base( "Eid Editor", "Eid" ) { }

        protected override EidManager GetNewManager() => new( this );
    }
}
