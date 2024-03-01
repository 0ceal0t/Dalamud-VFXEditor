using ImGuiNET;
using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Npc {
    public class NpcTabPap : NpcTab {
        public NpcTabPap( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected() {
            ImGui.Text( "Variant: " + Selected.Variant );
            DrawPaths( Loaded, Selected.Name );
        }

        protected override void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded ) {
            loaded = files.pap;
        }
    }
}