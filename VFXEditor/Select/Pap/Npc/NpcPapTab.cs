using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Pap.Npc {
    public class NpcPapTab : NpcTab {
        public NpcPapTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected() {
            ImGui.Text( "Variant: " + Selected.Variant );
            Dialog.DrawPaths( "PAP", Loaded, SelectResultType.GameNpc, Selected.Name );
        }

        protected override void FilesToSelected( NpcFilesStruct files, out List<string> selected ) {
            selected = files.pap;
        }
    }
}
