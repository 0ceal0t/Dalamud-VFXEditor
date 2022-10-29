using ImGuiNET;
using System;
using System.Numerics;
using VfxEditor.Dialogs;

namespace VfxEditor.PapFormat {
    public class PapSelectIndexDialog : GenericDialog {
        public Action<int> OnOk;
        public int Index;

        public PapSelectIndexDialog() : base( "Select Index" ) {
            Size = new Vector2( 400, 100 );
        }

        public override void DrawBody() {
            ImGui.SetNextItemWidth( 100f );
            ImGui.InputInt( "Index of the animation being imported##PapIndex", ref Index );

            if( ImGui.Button( "Ok##PapIndex" ) ) {
                OnOk?.Invoke( Index );
                Visible = false;
            }
            ImGui.SameLine();
            if( ImGui.Button( "Cancel##PapIndex" ) ) {
                Visible = false;
            }
        }
    }
}