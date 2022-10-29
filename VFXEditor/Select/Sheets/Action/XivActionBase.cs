namespace VfxEditor.Select.Rows {
    public abstract class XivActionBase {
        public string Name;
        public int RowId;
        public ushort Icon;

        public bool CastKeyExists = false;
        public bool SelfKeyExists = false;
        public bool HitKeyExists = false;
        public bool KeyExists = false;

        public XivActionBase HitAction = null;

        public string Castvfx;
        public string SelfKey;
        public string HitKey;

        public static readonly string castPrefix = "vfx/common/eff/";

        public string GetCastVFXPath() {
            if( !CastKeyExists ) return "";
            return castPrefix + Castvfx + ".avfx";
        }

        public string GetTmbPath() {
            return "chara/action/" + SelfKey + ".tmb";
        }
    }
}
