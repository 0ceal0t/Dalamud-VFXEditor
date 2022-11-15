using ImGuiNET;
using System;
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

        public override void Draw( string parentId ) {
            var id = parentId + "/Vnum";

            Number.Number.Draw( id, CommandManager.Avfx );
            Vertex.Position.Draw( id, CommandManager.Avfx );
            Vertex.Normal.Draw( id, CommandManager.Avfx );
            Vertex.Color.Draw( id, CommandManager.Avfx );
        }

        public override string GetDefaultText() => $"{GetIdx()}";
    }
}
