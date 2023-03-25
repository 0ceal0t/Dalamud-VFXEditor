using System;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Eid.Npc {
    public class NpcEidTab : NpcTab {
        public NpcEidTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected( string parentId ) {
            Dialog.DrawPath( "Path", Selected.GetEidPath(), parentId, SelectResultType.GameNpc, Selected.Name );
        }

        protected override void FilesToSelected( NpcFilesStruct files, out NpcRowSelected selected ) {
            selected = new( null ); // don't actually need any of the files
        }
    }
}
