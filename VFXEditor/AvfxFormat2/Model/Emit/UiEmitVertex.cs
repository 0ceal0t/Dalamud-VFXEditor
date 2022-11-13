using ImGuiNET;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiEmitVertex : GenericSelectableItem {
        public readonly AvfxEmiterVertex Vertex;
        public readonly AvfxVertexNumber Number;

        public UiEmitVertex( AvfxEmiterVertex vertex, AvfxVertexNumber number ) {
            Vertex = vertex;
            Number = number;
        }

        public override void Draw( string parentId ) {
            var id = parentId + "/Vnum";

            ImGui.InputInt( "Order" + id, ref Number.Num );
            ImGui.InputFloat3( "Position" + id, ref Vertex.Position );
            ImGui.InputFloat3( "Normal" + id, ref Vertex.Normal );
            ImGui.ColorEdit4( "Color" + id, ref Vertex.Color );
        }

        public override string GetDefaultText() => $"{GetIdx()}";
    }
}
