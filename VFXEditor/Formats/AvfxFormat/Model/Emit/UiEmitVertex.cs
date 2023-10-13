using OtterGui.Raii;
using System.Numerics;

namespace VfxEditor.AvfxFormat {
    public class UiEmitVertex : GenericSelectableItem {
        public readonly AvfxEmitVertex Vertex;
        public readonly AvfxVertexNumber Number;

        private readonly AvfxModel Model;

        public Vector3 Position => Vertex.Position.Value;
        public Vector3 Normal => Vertex.Normal.Value;
        public int Order => Number.Number.Value;

        public UiEmitVertex( AvfxModel model, AvfxEmitVertex vertex, AvfxVertexNumber number ) {
            Model = model;
            Vertex = vertex;
            Number = number;
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "VNum" );

            Number.Number.Draw( CommandManager.Avfx );
            Vertex.Position.Draw( CommandManager.Avfx, out var positionChanged );
            Vertex.Normal.Draw( CommandManager.Avfx, out var normalChanged );
            Vertex.Color.Draw( CommandManager.Avfx );

            if( positionChanged || normalChanged ) Model.RefreshModelPreview();
        }

        public override string GetDefaultText() => $"{GetIdx()}";
    }
}
