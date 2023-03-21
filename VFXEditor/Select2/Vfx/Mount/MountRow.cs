using VfxEditor.Select2.Shared.Npc;

namespace VfxEditor.Select2.Vfx.Mount
{
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
