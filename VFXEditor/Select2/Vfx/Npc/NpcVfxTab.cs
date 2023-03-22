using ImGuiNET;
using VfxEditor.Select2.Shared.Npc;

namespace VfxEditor.Select2.Vfx.Npc {
    public class NpcVfxTab : NpcTab {
        public NpcVfxTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "Variant: " + Selected.Variant );
            Dialog.DrawPath( "VFX", Loaded.Paths, parentId, SelectResultType.GameNpc, Selected.Name, true );
        }

        protected override void FilesToSelected( NpcFilesStruct files, out NpcRowSelected selected ) {
            selected = new( files.vfx );
        }
    }
}
