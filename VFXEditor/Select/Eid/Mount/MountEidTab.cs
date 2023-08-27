using VfxEditor.Select.Shared.Mount;

namespace VfxEditor.Select.Eid.Mount {
    public class MountEidTab : MountTab {
        public MountEidTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected() {
            SelectUiUtils.DrawIcon( Icon );

            DrawPath( "Path", Selected.GetEidPath(), Selected.Name );
        }
    }
}
