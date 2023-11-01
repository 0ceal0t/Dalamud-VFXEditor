using ImGuiNET;
using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Npc {
    public class NpcTabVfx : NpcTab {
        public NpcTabVfx( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected() {
            ImGui.Text( "Variant: " + Selected.Variant );
            DrawPaths( "VFX", Loaded, Selected.Name, true );
        }

        protected override void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded ) {
            loaded = files.vfx;
        }
    }
}