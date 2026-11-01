using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TmbFormat {
    public class TmbManagerGroup : FileManagerGroup<TmbManager, TmbDocument, TmbFile, WorkspaceMetaBasic> {
        public TmbManagerGroup() : base( "Tmb Editor", "Tmb" ) { }

        protected override TmbManager GetNewManager() => new( this );
    }
}
