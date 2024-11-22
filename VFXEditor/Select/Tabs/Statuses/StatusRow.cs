using Lumina.Excel.Sheets;
using System.Collections.Generic;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Statuses {
    public class StatusRow : ISelectItemWithIcon {
        public readonly string Name;
        public readonly int RowId;
        public readonly uint Icon;

        public readonly string HitPath;
        public readonly List<string> LoopPaths = [];

        public bool VfxExists => LoopPaths.Count > 0 || !string.IsNullOrEmpty( HitPath );

        public StatusRow( Status status ) {
            Name = status.Name.ToString();
            RowId = ( int )status.RowId;
            Icon = status.Icon;

            HitPath = GetVfxPath( status.HitEffect.ValueNullable?.Location.ValueNullable?.Location.ExtractText() );

            var loopVfxs = status.VFX.ValueNullable?.VFX;
            if( loopVfxs == null ) return;
            foreach( var vfx in loopVfxs ) {
                var key = vfx.ValueNullable?.Location.ExtractText();
                if( !string.IsNullOrEmpty( key ) ) LoopPaths.Add( GetVfxPath( key ) );
            }
        }

        private static string GetVfxPath( string path ) => string.IsNullOrEmpty( path ) ? "" : $"vfx/common/eff/{path}.avfx";

        public string GetName() => Name;

        public uint GetIconId() => Icon;
    }
}