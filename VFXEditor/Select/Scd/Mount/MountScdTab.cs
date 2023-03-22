using System.Linq;
using VfxEditor.Select.Shared.Mount;

namespace VfxEditor.Select.Scd.Mount {
    public class MountScdTab : MountTab {
        public MountScdTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPath( "Bgm", Selected.Bgm, parentId, SelectResultType.GameNpc, Selected.Name );
        }
    }
}
