namespace VfxEditor.Select.Tabs.Mounts {
    public class MountTabScd : MountTab<object> {
        public MountTabScd( SelectDialog dialog, string name ) : base( dialog, name ) { }

        public override void LoadSelection( MountRow item, out object loaded ) { loaded = new(); }

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            DrawPath( "Mount", Selected.Sound, $"{Selected.Name} Mount" );
            DrawPath( "Bgm", Selected.Bgm, $"{Selected.Name} BGM" );
        }
    }
}