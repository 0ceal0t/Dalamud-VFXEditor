using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public abstract class XivActionBase {
        public string Name;
        public int RowId;
        public ushort Icon;

        public bool CastVFXExists = false;
        public bool SelfVFXExists = false;
        public bool HitVFXExists = false;
        public bool VfxExists = false;

        public XivActionBase HitAction = null;

        public string CastVFX;
        public string SelfVFXKey;
        public string HitVFXKey;

        public static readonly string castPrefix = "vfx/common/eff/";

        public string GetCastVFXPath() {
            if( !CastVFXExists )
                return "--";
            return castPrefix + CastVFX + ".avfx";
        }

        public string GetTmbPath() {
            return "chara/action/" + SelfVFXKey + ".tmb";
        }
    }
}
