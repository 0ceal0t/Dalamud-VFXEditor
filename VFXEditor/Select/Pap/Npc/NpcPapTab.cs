using ImGuiNET;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Pap.Npc {
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
