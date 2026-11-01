using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AvfxFormat {
    public class AvfxManagerGroup : FileManagerGroup<AvfxManager, AvfxDocument, AvfxFile, WorkspaceMetaRenamed> {
        public AvfxManagerGroup() : base( "VFXEditor", "Vfx", "avfx", "Docs", "VFX" ) { }

        public override FormatCategory Category => FormatCategory.Vfx;

        protected override AvfxManager GetNewManager() => new( this );
    }
}