using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXSelect.Select.Rows {
    public class XivStatus {
        public string Name;
        public int RowId;
        public ushort Icon;

        public string LoopVFXPath1;
        public string LoopVFXPath2;
        public string LoopVFXPath3;
        public bool VfxExists = false;

        public static readonly string statusPrefix = "vfx/common/eff/";

        public XivStatus( Lumina.Excel.GeneratedSheets.Status status ) {
            Name = status.Name.ToString();
            RowId = ( int )status.RowId;
            Icon = status.Icon;

            //HitVFXPath = status.HitEffect.Value?.Location.Value?.Location;

            LoopVFXPath1 = GetVFXPath( status.VFX.Value?.VFX?.Value.Location );
            LoopVFXPath2 = GetVFXPath( status.VFX.Value?.VFX?.Value.Location );
            LoopVFXPath3 = GetVFXPath( status.VFX.Value?.VFX?.Value.Location );

            VfxExists = !string.IsNullOrEmpty( LoopVFXPath1 ) || !string.IsNullOrEmpty( LoopVFXPath2 ) || !string.IsNullOrEmpty( LoopVFXPath3 );
        }

        private static string GetVFXPath( string path ) {
            if( string.IsNullOrEmpty( path ) ) return "";
            return $"{statusPrefix}{path}.avfx";
        }
    }
}
