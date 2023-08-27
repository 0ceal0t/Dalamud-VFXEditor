namespace VfxEditor.Select.Vfx.Status {
    public class StatusRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly uint Icon;

        public readonly string HitVfxPath;
        public readonly string LoopVfxPath1;
        public readonly string LoopVfxPath2;
        public readonly string LoopVfxPath3;
        public readonly bool VfxExists = false;

        public static readonly string statusPrefix = "vfx/common/eff/";

        public StatusRow( Lumina.Excel.GeneratedSheets.Status status ) {
            Name = status.Name.ToString();
            RowId = ( int )status.RowId;
            Icon = status.Icon;

            HitVfxPath = GetVfxPath( status.HitEffect.Value?.Location.Value?.Location );
            LoopVfxPath1 = GetVfxPath( status.VFX.Value?.VFX?.Value.Location );
            LoopVfxPath2 = GetVfxPath( status.VFX.Value?.VFX2?.Value.Location );
            LoopVfxPath3 = GetVfxPath( status.VFX.Value?.VFX3?.Value.Location );

            VfxExists = !string.IsNullOrEmpty( LoopVfxPath1 ) || !string.IsNullOrEmpty( LoopVfxPath2 ) || !string.IsNullOrEmpty( LoopVfxPath3 ) || !string.IsNullOrEmpty( HitVfxPath );
        }

        private static string GetVfxPath( string path ) => string.IsNullOrEmpty( path ) ? "" : $"{statusPrefix}{path}.avfx";
    }
}
