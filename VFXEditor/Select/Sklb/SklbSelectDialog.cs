using VfxEditor.Select.Shared.Skeleton;
using VfxEditor.SklbFormat;

namespace VfxEditor.Select.Sklb {
    public class SklbSelectDialog : SelectDialog {
        public SklbSelectDialog( string id, SklbManager manager, bool isSourceDialog ) : base( id, "sklb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new SkeletonArmorTab( this, "Armor", "skl", "sklb" ),
                new SkeletonNpcTab( this, "Npc" , "skl", "sklb"),
                new SkeletonCharacterTab( this, "Character", "skl", "sklb", true ),
                new SkeletonMountTab( this, "Mount", "skl", "sklb")
            } );
        }
    }
}
