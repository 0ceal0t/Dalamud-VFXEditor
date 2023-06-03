using System.Collections.Generic;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Shared.Mount {
    public class MountRow : NpcRow {
        public readonly ushort Icon;
        public readonly string Bgm;
        public readonly int Seats;
        public readonly int SEPack;

        public MountRow( Lumina.Excel.GeneratedSheets.Mount mount ) : base( mount.ModelChara.Value ) {
            Icon = mount.Icon;
            Name = mount.Singular.ToString();
            Bgm = mount.RideBGM?.Value.File.ToString();
            Seats = mount.ExtraSeats + 1;
            SEPack = mount.ModelChara?.Value.SEPack ?? 0;
        }

        // chara/human/[character ID]/animation/a0001/mt-[monster ID]/resident/mount.pap
        // chara/human/c0101/animation/a0001/mt_m0457/resident/mount01.pap

        // chara/monster/m0536/animation/a0001/bt_common/resident/mount.pap
        // sound/battle/mon/3388.scd

        public string GetMountSound() => $"sound/battle/mon/{SEPack}.scd";

        public string GetMountPap() => "chara/" + ( Type == NpcType.Monster ? "monster/" : "demihuman/" ) + ModelString +
            "/animation/a0001/bt_common/resident/mount.pap";

        public List<string> GetMountSeatPaps() {
            var ret = new List<string>();
            if( Seats == 1 ) ret.Add( $"mt_{ModelString}/resident/mount.pap" );
            else {
                for( var i = 1; i <= Seats; i++ ) ret.Add( $"mt_{ModelString}/resident/mount0{i}.pap" );
            }
            return ret;
        }
    }
}
