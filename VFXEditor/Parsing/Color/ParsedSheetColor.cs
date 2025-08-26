using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.IO;
using System.Numerics;
using VfxEditor.Data;

namespace VfxEditor.Parsing.Color {
    public class ParsedSheetColor : ParsedSimpleBase<uint> {
        private Vector4 CurrentColor => SheetData.UiColors.TryGetValue( Value, out var color ) ? color : new Vector4( 1, 1, 1, 1 );

        public ParsedSheetColor( string name ) : base( name ) {
            SheetData.InitUiColors();
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader, 0 );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadUInt32();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        protected override void DrawBody() {
            DrawCombo();
            ImGui.SameLine();
            ImGui.Text( Name );
        }

        private void DrawCombo() {
            using var color = ImRaii.PushColor( ImGuiCol.Text, CurrentColor );
            var text = SheetData.UiColors.ContainsKey( Value ) ? $"----{Value}----" : "[NONE]";

            using var combo = ImRaii.Combo( $"##Combo{Name}", text );
            if( !combo ) return;

            foreach( var option in SheetData.UiColors ) {
                var selected = option.Key == Value;

                using var _ = ImRaii.PushColor( ImGuiCol.Text, option.Value );
                if( ImGui.Selectable( $"----{option.Key}----", selected ) ) {
                    Update( option.Key );
                }

                if( selected ) ImGui.SetItemDefaultFocus();
            }
        }
    }
}
