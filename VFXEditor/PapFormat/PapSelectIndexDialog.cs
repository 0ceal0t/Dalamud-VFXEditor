using ImGuiNET;
using OtterGui.Raii;
using System;
using VfxEditor.Ui;

namespace VfxEditor.PapFormat {
    public class PapSelectIndexDialog : GenericDialog {
        public Action<int> OnOk;
        public int Index;

        public PapSelectIndexDialog() : base( "Select Index", false, 400, 100 ) { }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( "PapIndex" );

            ImGui.SetNextItemWidth( 100f );
            ImGui.InputInt( "Index of the animation being imported", ref Index );

            if( ImGui.Button( "Ok" ) ) {
                OnOk?.Invoke( Index );
                Visible = false;
            }
            ImGui.SameLine();
            if( ImGui.Button( "Cancel" ) ) {
                Visible = false;
            }
        }
    }
}