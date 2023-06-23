using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Vfx.Npc {
    public class NpcVfxTab : NpcTab {
        public NpcVfxTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected() {
            ImGui.Text( "Variant: " + Selected.Variant );
            Dialog.DrawPaths( "VFX", Loaded, SelectResultType.GameNpc, Selected.Name, true );
        }

        protected override void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded ) {
            loaded = files.vfx;
        }
    }
}
