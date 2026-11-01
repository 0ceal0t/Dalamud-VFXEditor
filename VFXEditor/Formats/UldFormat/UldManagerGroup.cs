using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.UldFormat {
    public class UldManagerGroup : FileManagerGroup<UldManager, UldDocument, UldFile, WorkspaceMetaRenamed> {
        public UldManagerGroup() : base( "Uld Editor", "Uld" ) { }

        protected override UldManager GetNewManager() => new( this );
    }
}
