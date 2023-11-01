using VfxEditor.Select.Tabs.Character;
using VfxEditor.Select.Tabs.Skeleton;
using VfxEditor.SklbFormat;

namespace VfxEditor.Select.Formats {
    public class SklbSelectDialog : SelectDialog {
        public SklbSelectDialog( string id, SklbManager manager, bool isSourceDialog ) : base( id, "sklb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new SkeletonTabArmor( this, "Armor", "skl", "sklb" ),
                new SkeletonTabNpc( this, "Npc" , "skl", "sklb"),
                new CharacterTabSkeleton( this, "Character", "skl", "sklb", true ),
                new SkeletonTabMount( this, "Mount", "skl", "sklb")
            } );
        }
    }
}