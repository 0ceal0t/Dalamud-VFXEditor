using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor.Parsing.Sheets {
    public class ParsedSubtitle : ParsedSimpleBase<int> {
        private string CurrentId => SheetData.Subtitle.TryGetValue( Value, out var Id ) ? Id : "";

        public ParsedSubtitle( string name ) : base( name ) {
            SheetData.InitSubtitle();
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadInt32();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        protected override void DrawBody() {
            DrawCombo();
            ImGui.SameLine();
            ImGui.Text( Name );
        }

        private void DrawCombo() {
            using var combo = ImRaii.Combo( "##Combo", $"[{Value}] {CurrentId}" );
            if( !combo ) return;

            foreach( var option in SheetData.Subtitle ) {
                var selected = option.Key == Value;

                if( ImGui.Selectable( $"[{option.Key}] {option.Value}", selected ) ) {
                    Update( option.Key );
                }

                if( selected ) ImGui.SetItemDefaultFocus();
            }
        }
    }
}
