using VfxEditor.Select.Tabs.Mounts;

namespace VfxEditor.Select.Tabs.Skeleton {
    public class SkeletonTabMount : MountTab<object> {
        private readonly string Prefix;
        private readonly string Extension;

        public SkeletonTabMount( SelectDialog dialog, string name, string prefix, string extension ) : base( dialog, name ) {
            Prefix = prefix;
            Extension = extension;
        }

        public override void LoadSelection( MountRow item, out object loaded ) { loaded = new(); }

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            var path = Selected.GetSkeletonPath( Prefix, Extension );
            if( Dalamud.DataManager.FileExists( path ) ) {
                DrawPath( "Path", path, Selected.Name );
            }
        }
    }
}
