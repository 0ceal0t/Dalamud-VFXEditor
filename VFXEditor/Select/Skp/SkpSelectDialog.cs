using VfxEditor.Formats.SkpFormat;
using VfxEditor.Select.Shared.Skeleton;

namespace VfxEditor.Select.Skp {
    public class SkpSelectDialog : SelectDialog {
        public SkpSelectDialog( string id, SkpManager manager, bool isSourceDialog ) : base( id, "skp", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new SkeletonNpcTab( this, "Npc", "skl", "skp"),
                new SkeletonCharacterTab( this, "Character", "skl", "skp", false )
            } );
        }
    }
}
