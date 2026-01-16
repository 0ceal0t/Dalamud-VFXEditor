using Dalamud.Bindings.ImGui;
using System.Numerics;
using VfxEditor.Data.Command;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiEmitVertex : IUiItem {
        public readonly AvfxEmitVertex Vertex;

        private readonly AvfxModel Model;

        public Vector3 Position => Vertex.Position.Value;
        public Vector3 Normal => Vertex.Normal.Value;

        public UiEmitVertex( AvfxModel model, AvfxEmitVertex vertex) {
            Model = model;
            Vertex = vertex;
        }

        public void Draw() {
            using var edited = new Edited();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 170 );
            Vertex.Position.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 170 );
            Vertex.Normal.Draw();

            ImGui.TableNextColumn();
            Vertex.Color.Draw();

            if( edited.IsEdited ) Model.Updated();
        }
    }
}
