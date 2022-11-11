using ImGuiNET;
using System.Numerics;
using VfxEditor.AVFXLib.Model;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiModelEmitterVertex : UiItem {
        public UiModel Model;
        public AVFXVertexNumber VertexNumber;
        public AVFXEmitVertex Vertex;
        public int Order;
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 Color;

        public UiModelEmitterVertex( AVFXVertexNumber vnum, AVFXEmitVertex emitVertex, UiModel model ) {
            Model = model;
            VertexNumber = vnum;
            Vertex = emitVertex;

            Order = VertexNumber.Num;
            Position = new Vector3( Vertex.Position[0], Vertex.Position[1], Vertex.Position[2] );
            Normal = new Vector3( Vertex.Normal[0], Vertex.Normal[1], Vertex.Normal[2] );
            Color = GltfUtils.IntToColor( Vertex.C ) / 255;
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/VNum";
            if( ImGui.InputInt( "Order" + id, ref Order ) ) {
                VertexNumber.Num = Order;
            }
            if( ImGui.InputFloat3( "Position" + id, ref Position ) ) {
                Vertex.Position = new float[] { Position.X, Position.Y, Position.Z };
            }
            if( ImGui.InputFloat3( "Normal" + id, ref Normal ) ) {
                Vertex.Normal = new float[] { Normal.X, Normal.Y, Normal.Z };
            }
            if( ImGui.ColorEdit4( "Color" + id, ref Color ) ) {
                Vertex.C = GltfUtils.ColorToInt( Color * 255 );
            }
        }

        public override string GetDefaultText() => $"{Idx}";
    }
}
