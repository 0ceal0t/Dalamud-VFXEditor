using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivStatus
    {
        public bool LoopVFX1Exists = false;
        public bool LoopVFX2Exists = false;
        public bool LoopVFX3Exists = false;
        public bool VfxExists = false;

        public string Name;
        public int RowId;
        public ushort Icon;

        public string LoopVFXPath1;
        public string LoopVFXPath2;
        public string LoopVFXPath3;

        public static string statusPrefix = "vfx/common/eff/";

        public XivStatus( Lumina.Excel.GeneratedSheets.Status status )
        {
            Name = status.Name.ToString();
            RowId = ( int )status.RowId;
            Icon = status.Icon;

            //HitVFXPath = status.HitEffect.Value?.Location.Value?.Location;

            LoopVFXPath1 = status.VFX.Value?.VFX?.Value.Location;
            LoopVFX1Exists = !string.IsNullOrEmpty( LoopVFXPath1 );

            LoopVFXPath2 = status.VFX.Value?.VFX2?.Value.Location;
            LoopVFX2Exists = !string.IsNullOrEmpty( LoopVFXPath2 );

            LoopVFXPath3 = status.VFX.Value?.VFX3?.Value.Location;
            LoopVFX3Exists = !string.IsNullOrEmpty( LoopVFXPath3 );

            VfxExists = (LoopVFX1Exists || LoopVFX2Exists || LoopVFX3Exists);
        }

        public string GetLoopVFX1Path()
        {
            if( !LoopVFX1Exists )
                return "--";
            return statusPrefix + LoopVFXPath1 + ".avfx";
        }

        public string GetLoopVFX2Path()
        {
            if( !LoopVFX2Exists )
                return "--";
            return statusPrefix + LoopVFXPath2 + ".avfx";
        }

        public string GetLoopVFX3Path()
        {
            if( !LoopVFX3Exists )
                return "--";
            return statusPrefix + LoopVFXPath3 + ".avfx";
        }
    }
}
