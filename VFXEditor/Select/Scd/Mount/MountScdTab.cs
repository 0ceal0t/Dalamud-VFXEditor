using System.Linq;
using VfxEditor.Select.Shared.Mount;

namespace VfxEditor.Select.Scd.Mount {
    public class MountScdTab : MountTab {
        public MountScdTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected() {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPath( "Bgm", Selected.Bgm, SelectResultType.GameNpc, Selected.Name );
        }
    }
}
