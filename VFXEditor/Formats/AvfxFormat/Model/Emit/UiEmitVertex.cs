using ImGuiNET;
using System.Numerics;
using VfxEditor.Data.Command;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiEmitVertex : IUiItem {
        public readonly AvfxEmitVertex Vertex;
        public readonly AvfxVertexNumber Number;

        private readonly AvfxModel Model;

        public Vector3 Position => Vertex.Position.Value;
        public Vector3 Normal => Vertex.Normal.Value;
        public int Order => Number.Order.Value;

        public UiEmitVertex( AvfxModel model, AvfxEmitVertex vertex, AvfxVertexNumber number ) {
            Model = model;
            Vertex = vertex;
            Number = number;
        }

        public void Draw() {
            using var edited = new Edited();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 75 );
            Number.Order.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 200 );
            Vertex.Position.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 200 );
            Vertex.Normal.Draw();

            ImGui.TableNextColumn();
            Vertex.Color.Draw();

            if( edited.IsEdited ) Model.RefreshModelPreview();
        }
    }
}
