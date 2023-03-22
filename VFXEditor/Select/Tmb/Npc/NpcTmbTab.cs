using ImGuiNET;
using VfxEditor.Select.Shared.Npc;

namespace VfxEditor.Select.Tmb.Npc {
    public class NpcTmbTab : NpcTab {
        public NpcTmbTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "Variant: " + Selected.Variant );
            Dialog.DrawPath( "TMB", Loaded.Paths, parentId, SelectResultType.GameNpc, Selected.Name );
        }

        protected override void FilesToSelected( NpcFilesStruct files, out NpcRowSelected selected ) {
            selected = new( files.tmb );
        }
    }
}
