using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Pap.Npc {
    public class NpcPapTab : NpcTab {
        public NpcPapTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "Variant: " + Selected.Variant );
            Dialog.DrawPath( "PAP", Loaded, parentId, SelectResultType.GameNpc, Selected.Name );
        }

        protected override void FilesToSelected( NpcFilesStruct files, out List<string> selected ) {
            selected = files.pap;
        }
    }
}
