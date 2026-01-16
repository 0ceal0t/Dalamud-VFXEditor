using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;
using VfxEditor.DirectX.Model;

namespace VfxEditor.DirectX.Lights {
    [Serializable]
    public class LightConfiguration {
        public Vector3 Position;
        public Vector3 Color;
        public float Radius;
        public float Falloff;

        public LightConfiguration( Vector3 position, Vector3 color, float radius, float falloff ) {
            Position = position;
            Color = color;
            Radius = radius;
            Falloff = falloff;
        }

        public LightData GetData() => new() {
            Position = Position,
            Color = Color,
            Radius = Radius,
            Falloff = Falloff
        };

        public bool Draw() {
            var updated = false;

            updated |= ImGui.InputFloat3( "Light Position", ref Position );
            updated |= ImGui.ColorEdit3( "Light Color", ref Color );
            updated |= ImGui.InputFloat( "Light Radius", ref Radius );
            updated |= ImGui.InputFloat( "Light Falloff", ref Falloff );

            return updated;
        }
    }
}
