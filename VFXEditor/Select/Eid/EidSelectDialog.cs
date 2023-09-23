using VfxEditor.EidFormat;
using VfxEditor.Select.Shared.Skeleton;

namespace VfxEditor.Select.Eid {
    public class EidSelectDialog : SelectDialog {
        public EidSelectDialog( string id, EidManager manager, bool isSourceDialog ) : base( id, "eid", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new SkeletonNpcTab( this, "Npc", "eid", "eid"),
                new SkeletonCharacterTab( this, "Character", "eid", "eid", false ),
                new SkeletonMountTab( this, "Mount", "eid", "eid"),
            } );
        }
    }
}
