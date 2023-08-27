using VfxEditor.FileManager;
using VfxEditor.Select.Shared.Skeleton;

namespace VfxEditor.Select.Phyb {
    public class PhybSelectDialog : SelectDialog {
        public PhybSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "phyb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new SkeletonArmorTab( this, "Armor", "phy", "phyb" ),
                new SkeletonNpcTab( this, "Npc" , "phy", "phyb"),
                new SkeletonCharacterTab( this, "Character", "phy", "phyb" ),
                new SkeletonMountTab( this, "Mount", "phy", "phyb")
            } );
        }
    }
}
