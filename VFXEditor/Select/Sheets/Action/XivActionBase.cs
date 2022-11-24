namespace VfxEditor.Select.Rows {
    public abstract class XivActionBase {
        public static readonly string CastPrefix = "vfx/common/eff/";

        public string Name;
        public int RowId;
        public ushort Icon;
        public XivActionBase HitAction = null;
        public string CastVfxKey;
        public string SelfTmbKey;

        public string CastVfxPath => string.IsNullOrEmpty( CastVfxKey ) ? "" : $"{CastPrefix}{CastVfxKey}.avfx";
        public bool HasVfx => !string.IsNullOrEmpty( CastVfxKey ) || !string.IsNullOrEmpty( SelfTmbKey );
        public string TmbPath => $"chara/action/{SelfTmbKey}.tmb";
    }
}
