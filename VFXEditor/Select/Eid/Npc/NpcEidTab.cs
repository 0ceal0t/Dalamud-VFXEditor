using System.Collections.Generic;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Eid.Npc {
    public class NpcEidTab : NpcTab {
        public NpcEidTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected() {
            Dialog.DrawPath( "Path", Selected.GetEidPath(), SelectResultType.GameNpc, Selected.Name );
        }

        protected override void FilesToSelected( NpcFilesStruct files, out List<string> selected ) {
            selected = new List<string>();
        }
    }
}
