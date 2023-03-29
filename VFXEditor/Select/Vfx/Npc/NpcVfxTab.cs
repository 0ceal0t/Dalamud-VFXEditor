using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Vfx.Npc {
    public class NpcVfxTab : NpcTab {
        public NpcVfxTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "Variant: " + Selected.Variant );
            Dialog.DrawPath( "VFX", Loaded, parentId, SelectResultType.GameNpc, Selected.Name, true );
        }

        protected override void FilesToSelected( NpcFilesStruct files, out List<string> selected ) {
            selected = files.vfx;
        }
    }
}
