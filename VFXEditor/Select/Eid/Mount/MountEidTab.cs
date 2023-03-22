using VfxEditor.Select.Shared.Mount;

namespace VfxEditor.Select.Eid.Mount {
    public class MountEidTab : MountTab {
        public MountEidTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPath( "Path", Selected.GetEidPath(), parentId, SelectResultType.GameNpc, Selected.Name );
        }
    }
}
