using Lumina.Excel.GeneratedSheets2;
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
            // Icon = ( uint )status.Icon;
            Icon = status.ParamModifier; // temp until it's fixed
            // https://github.com/NotAdam/Lumina.Excel/commit/17f027682a3ff6b2d565c3c8d15d6f3146abd556#diff-917643f11f9c3f4ee19f19e4adb28e085d259d29322636b41d5c73fde427a510

            HitPath = GetVfxPath( status.HitEffect.Value?.Location.Value?.Location );

            var loopVfxs = status.VFX.Value?.VFX;
            if( loopVfxs == null ) return;
            foreach( var vfx in loopVfxs ) {
                if( vfx?.Value == null ) continue;
                var key = vfx?.Value.Location;
                if( string.IsNullOrEmpty( key ) ) continue;
                LoopPaths.Add( GetVfxPath( key ) );
            }
        }

        private static string GetVfxPath( string path ) => string.IsNullOrEmpty( path ) ? "" : $"vfx/common/eff/{path}.avfx";

        public string GetName() => Name;

        public uint GetIconId() => Icon;
    }
}