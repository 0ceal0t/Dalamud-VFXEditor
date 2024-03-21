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
            var path = Selected.GetSkeletonPath( Prefix, Extension );
            if( Dalamud.DataManager.FileExists( path ) ) {
                Dialog.DrawPaths( path, Selected.Name, SelectResultType.GameMount );
            }
        }
    }
}
