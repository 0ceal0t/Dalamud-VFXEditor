using VfxEditor.PhybFormat;
using VfxEditor.Select.Tabs.Character;
using VfxEditor.Select.Tabs.Skeleton;

namespace VfxEditor.Select.Formats {
    public class PhybSelectDialog : SelectDialog {
        public PhybSelectDialog( string id, PhybManager manager, bool isSourceDialog ) : base( id, "phyb", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new SkeletonTabArmor( this, "Armor", "phy", "phyb" ),
                new SkeletonTabNpc( this, "Npc" , "phy", "phyb"),
                new CharacterTabSkeleton( this, "Character", "phy", "phyb", true ),
                new SkeletonTabMount( this, "Mount", "phy", "phyb")
            ] );
        }
    }
}