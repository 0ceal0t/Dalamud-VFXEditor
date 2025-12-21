using Dalamud.Bindings.ImGui;
using System.Numerics;
using VfxEditor.Data.Command;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiVertexNumber : IUiItem {
        public readonly AvfxVertexNumber Number;

        public UiVertexNumber( AvfxVertexNumber number ) {
            Number = number;
        }

        public void Draw() {
            using var edited = new Edited();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 50 );
            Number.Order.Draw();

        }
    }
}
