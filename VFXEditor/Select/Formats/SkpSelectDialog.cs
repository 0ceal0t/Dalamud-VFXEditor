using VfxEditor.Formats.SkpFormat;
using VfxEditor.Select.Tabs.Character;
using VfxEditor.Select.Tabs.Skeleton;

namespace VfxEditor.Select.Formats {
    public class SkpSelectDialog : SelectDialog {
        public SkpSelectDialog( string id, SkpManager manager, bool isSourceDialog ) : base( id, "skp", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new SkeletonTabNpc( this, "Npc", "skl", "skp"),
                new CharacterTabSkeleton( this, "Character", "skl", "skp", false )
            } );
        }
    }
}