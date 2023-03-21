namespace VfxEditor.Select.Rows {
    public class XivStatus {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;

        public readonly string HitVfxPath;
        public readonly string LoopVfxPath1;
        public readonly string LoopVfxPath2;
        public readonly string LoopVfxPath3;
        public readonly bool VfxExists = false;

        public static readonly string statusPrefix = "vfx/common/eff/";

        public XivStatus( Lumina.Excel.GeneratedSheets.Status status ) {
            Name = status.Name.ToString();
            RowId = ( int )status.RowId;
            Icon = status.Icon;

            HitVfxPath = GetVFXPath( status.HitEffect.Value?.Location.Value?.Location );
            LoopVfxPath1 = GetVFXPath( status.VFX.Value?.VFX?.Value.Location );
            LoopVfxPath2 = GetVFXPath( status.VFX.Value?.VFX2?.Value.Location );
            LoopVfxPath3 = GetVFXPath( status.VFX.Value?.VFX3?.Value.Location );

            VfxExists = !string.IsNullOrEmpty( LoopVfxPath1 ) || !string.IsNullOrEmpty( LoopVfxPath2 ) || !string.IsNullOrEmpty( LoopVfxPath3 ) || !string.IsNullOrEmpty( HitVfxPath );
        }

        private static string GetVFXPath( string path ) {
            if( string.IsNullOrEmpty( path ) ) return "";
            return $"{statusPrefix}{path}.avfx";
        }
    }
}
