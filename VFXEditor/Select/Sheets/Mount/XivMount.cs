namespace VfxEditor.Select.Rows {
    public class XivMount : XivNpc {
        public readonly ushort Icon;
        public readonly string Bgm;

        public XivMount( Lumina.Excel.GeneratedSheets.Mount mount ) : base( mount.ModelChara.Value ) {
            Icon = mount.Icon;
            Name = mount.Singular.ToString();
            Bgm = mount.RideBGM?.Value.File.ToString();
        }
    }
}
