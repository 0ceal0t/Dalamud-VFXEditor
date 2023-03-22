using ImGuiNET;
using VfxEditor.Select2.Shared.Npc;

namespace VfxEditor.Select2.Pap.Npc {
    public class NpcPapTab : NpcTab {
        public NpcPapTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "Variant: " + Selected.Variant );
            Dialog.DrawPath( "PAP", Loaded.Paths, parentId, SelectResultType.GameNpc, Selected.Name );
        }

        protected override void FilesToSelected( NpcFilesStruct files, out NpcRowSelected selected ) {
            selected = new( files.pap );
        }
    }
}
