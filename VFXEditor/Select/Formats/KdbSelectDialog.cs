using VfxEditor.Formats.KdbFormat;
using VfxEditor.Select.Tabs.Character;
using VfxEditor.Select.Tabs.Skeleton;

namespace VfxEditor.Select.Formats {
    public class KdbSelectDialog : SelectDialog {
        public KdbSelectDialog( string id, KdbManager manager, bool isSourceDialog ) : base( id, "kdb", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new SkeletonTabArmor( this, "Armor", "kdi", "kdb" ),
                new SkeletonTabWeapon( this, "Weapon", "kdi", "kdb" ),
                new SkeletonTabNpc( this, "Npc" , "kdi", "kdb"),
                new CharacterTabSkeleton( this, "Character", "kdi", "kdb", true ),
                new SkeletonTabMount( this, "Mount", "kdi", "kdb")
            ] );
        }
    }
}
