using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Shared.Mount {
    public class MountRow : NpcRow {
        public readonly ushort Icon;
        public readonly string Bgm;

        public MountRow( Lumina.Excel.GeneratedSheets.Mount mount ) : base( mount.ModelChara.Value ) {
            Icon = mount.Icon;
            Name = mount.Singular.ToString();
            Bgm = mount.RideBGM?.Value.File.ToString();
        }
    }
}
