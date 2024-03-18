using VfxEditor.EidFormat;
using VfxEditor.Select.Tabs.Character;
using VfxEditor.Select.Tabs.Skeleton;

namespace VfxEditor.Select.Formats {
    public class EidSelectDialog : SelectDialog {
        public EidSelectDialog( string id, EidManager manager, bool isSourceDialog ) : base( id, "eid", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new SkeletonTabNpc( this, "Npc", "eid", "eid"),
                new CharacterTabSkeleton( this, "Character", "eid", "eid", false ),
                new SkeletonTabMount( this, "Mount", "eid", "eid"),
            ] );
        }
    }
}