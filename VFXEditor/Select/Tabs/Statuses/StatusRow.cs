using Lumina.Excel.GeneratedSheets;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Statuses {
    public class StatusRow : ISelectItemWithIcon {
        public readonly string Name;
        public readonly int RowId;
        public readonly uint Icon;

        public readonly string HitKey;
        public readonly string LoopKey1;
        public readonly string LoopKey2;
        public readonly string LoopKey3;

        public bool VfxExists => !string.IsNullOrEmpty( LoopKey1 ) || !string.IsNullOrEmpty( LoopKey2 ) || !string.IsNullOrEmpty( LoopKey3 ) || !string.IsNullOrEmpty( HitKey );

        public string HitPath => GetVfxPath( HitKey );
        public string LoopPath1 => GetVfxPath( LoopKey1 );
        public string LoopPath2 => GetVfxPath( LoopKey2 );
        public string LoopPath3 => GetVfxPath( LoopKey3 );

        public StatusRow( Status status ) {
            Name = status.Name.ToString();
            RowId = ( int )status.RowId;
            Icon = status.Icon;

            HitKey = status.HitEffect.Value?.Location.Value?.Location;
            LoopKey1 = status.VFX.Value?.VFX?.Value.Location;
            LoopKey2 = status.VFX.Value?.VFX2?.Value.Location;
            LoopKey3 = status.VFX.Value?.VFX3?.Value.Location;
        }

        private static string GetVfxPath( string path ) => string.IsNullOrEmpty( path ) ? "" : $"vfx/common/eff/{path}.avfx";

        public string GetName() => Name;
        public uint GetIconId() => Icon;
    }
}