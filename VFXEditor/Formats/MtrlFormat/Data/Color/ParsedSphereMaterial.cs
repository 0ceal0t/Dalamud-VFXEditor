using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.MtrlFormat.Data.Color {
    public class ParsedSphereMaterial : ParsedShort {
        public ParsedSphereMaterial( string name ) : base( name ) { }

        public ParsedSphereMaterial( string name, int value ) : base( name, value ) { }

        protected override void DrawBody() {
            var text = ( Value >= 0 && Value < Plugin.MtrlManager.Sphere.Count ) ? $"Sphere {Value}" : "[UNKNOWN]";
            using var combo = ImRaii.Combo( Name, text );
            if( !combo ) return;

            for( var i = 0; i < Plugin.MtrlManager.Sphere.Count; i++ ) {
                if( ImGui.Selectable( $"Sphere {i}", i == Value ) ) Update( i );
                if( i == Value ) ImGui.SetItemDefaultFocus();

                if( ImGui.IsItemHovered() ) {
                    ImGui.BeginTooltip();
                    var sphere = Plugin.MtrlManager.Sphere[i];
                    ImGui.Image( sphere.Handle, new Vector2( 100, 100 ) );
                    ImGui.EndTooltip();
                }
            }
        }
    }
}
