using OtterGui.Raii;
using System.Numerics;

namespace VfxEditor.AvfxFormat {
    public class UiEmitVertex : GenericSelectableItem {
        public readonly AvfxEmitVertex Vertex;
        public readonly AvfxVertexNumber Number;

        public Vector3 Position => Vertex.Position.Value;
        public Vector3 Normal => Vertex.Normal.Value;
        public int Order => Number.Number.Value;

        public UiEmitVertex( AvfxEmitVertex vertex, AvfxVertexNumber number ) {
            Vertex = vertex;
            Number = number;
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "VNum" );

            Number.Number.Draw( CommandManager.Avfx );
            Vertex.Position.Draw( CommandManager.Avfx );
            Vertex.Normal.Draw( CommandManager.Avfx );
            Vertex.Color.Draw( CommandManager.Avfx );
        }

        public override string GetDefaultText() => $"{GetIdx()}";
    }
}
