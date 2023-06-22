using VfxEditor.FileManager;

namespace VfxEditor.Select.Phyb {
    public class PhybSelectDialog : SelectDialog {
        public PhybSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "phyb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
            } );
        }


        // ITEMS
        //  weapon
        //  items (race-specific)
        // NPC
        // CHARACTER
        // faces

        /*
         * chara/human/c1201/skeleton/hair/h0101/phy_c1201h0101.phyb
         * 
         * chara/human/c0801/skeleton/met/m6173/phy_c0801m6173.phyb
         * 
         * chara/monster/m0805/skeleton/base/b0001/phy_m0805b0001.phyb
         * 
         * chara/human/c0801/skeleton/top/t0748/phy_c0801t0748.phyb
         * 
         * chara/weapon/w9079/skeleton/base/b0001/phy_w9079b0001.phyb
         * 
         * chara/human/c1801/skeleton/face/f0005/phy_c1801f0005.phyb
         * 
         * chara/demihuman/d1063/skeleton/base/b0001/phy_d1063b0001.phyb
         * 
         * met = head
         * top = body
         */
    }
}
