using VfxEditor.FileManager;
using VfxEditor.Select.Shared.Skeleton;

namespace VfxEditor.Select.Sklb {
    public class SklbSelectDialog : SelectDialog {
        public SklbSelectDialog( string id, FileManagerBase manager, bool isSourceDialog ) : base( id, "sklb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new SkeletonArmorTab( this, "Armor", "skl", "sklb" ),
                new SkeletonNpcTab( this, "Npc" , "skl", "sklb"),
                new SkeletonCharacterTab( this, "Character", "skl", "sklb" ),
                new SkeletonMountTab( this, "Mount", "skl", "sklb")
            } );
        }
    }
}
