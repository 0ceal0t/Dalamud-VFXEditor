using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Mounts {
    public class MountTabScd : MountTab<object> {
        public MountTabScd( SelectDialog dialog, string name ) : base( dialog, name ) { }

        public override void LoadSelection( MountRow item, out object loaded ) { loaded = new(); }

        protected override void DrawSelected() {
            Dialog.DrawPaths( new Dictionary<string, string>() {
                { "Mount", Selected.Sound },
                { "Bgm", Selected.Bgm },
            }, Selected.Name, SelectResultType.GameMount );
        }
    }
}