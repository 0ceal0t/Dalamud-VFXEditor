using ImGuiNET;
using OtterGui.Raii;
using System;
using System.IO;
using System.Numerics;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class ParsedTileMaterial : ParsedInt {
        public ParsedTileMaterial( string name, int size = 4 ) : base( name, size ) { }

        public ParsedTileMaterial( string name, int value, int size = 4 ) : base( name, value, size ) { }

        public override void Read( BinaryReader reader ) {
            Value = ( int )( ( float )reader.ReadHalf() * 64f );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( Half )( ( Value + 0.5f ) / 64f ) );
        }

        protected override void DrawBody() {
            var size = Math.Min( Plugin.TextureManager.TileNormal.Count, Plugin.TextureManager.TileDiffuse.Count );

            var text = ( Value >= 0 && Value < size ) ? $"Tile {Value}" : "[UNKNOWN]";
            using var combo = ImRaii.Combo( Name, text );
            if( !combo ) return;

            for( var i = 0; i < size; i++ ) {
                if( ImGui.Selectable( $"Tile {i}", i == Value ) ) {
                    SetValue( i );
                }
                if( ImGui.IsItemHovered() ) {
                    ImGui.BeginTooltip();
                    var diffuse = Plugin.TextureManager.TileDiffuse[i];
                    var normal = Plugin.TextureManager.TileNormal[i];
                    ImGui.Image( diffuse.ImGuiHandle, new Vector2( 100, 100 ) );
                    ImGui.SameLine();
                    ImGui.Image( normal.ImGuiHandle, new Vector2( 100, 100 ) );
                    ImGui.EndTooltip();
                }
            }
        }
    }
}
