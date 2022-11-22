namespace VfxEditor.Select.Rows {
    public class XivStatus {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;

        public readonly string HitVFXPath;
        public readonly string LoopVFXPath1;
        public readonly string LoopVFXPath2;
        public readonly string LoopVFXPath3;
        public readonly bool VfxExists = false;

        public static readonly string statusPrefix = "vfx/common/eff/";

        public XivStatus( Lumina.Excel.GeneratedSheets.Status status ) {
            Name = status.Name.ToString();
            RowId = ( int )status.RowId;
            Icon = status.Icon;

            HitVFXPath = GetVFXPath( status.HitEffect.Value?.Location.Value?.Location );
            LoopVFXPath1 = GetVFXPath( status.VFX.Value?.VFX?.Value.Location );
            LoopVFXPath2 = GetVFXPath( status.VFX.Value?.VFX2?.Value.Location );
            LoopVFXPath3 = GetVFXPath( status.VFX.Value?.VFX3?.Value.Location );

            VfxExists = !string.IsNullOrEmpty( LoopVFXPath1 ) || !string.IsNullOrEmpty( LoopVFXPath2 ) || !string.IsNullOrEmpty( LoopVFXPath3 ) || !string.IsNullOrEmpty( HitVFXPath );
        }

        private static string GetVFXPath( string path ) {
            if( string.IsNullOrEmpty( path ) ) return "";
            return $"{statusPrefix}{path}.avfx";
        }
    }
}
