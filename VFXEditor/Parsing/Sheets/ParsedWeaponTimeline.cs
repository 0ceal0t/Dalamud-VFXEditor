using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor.Parsing.Sheets {
    public class ParsedWeaponTimeline : ParsedSimpleBase<ushort> {
        private string CurrentTimeline => SheetData.WeaponTimelines.TryGetValue( Value, out var timeline ) ? timeline : "";

        public ParsedWeaponTimeline( string name ) : base( name ) {
            SheetData.InitWeaponTimelines();
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader, 0 );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadUInt16();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        protected override void DrawBody( CommandManager manager ) {
            DrawCombo( manager );
            ImGui.SameLine();
            ImGui.Text( Name );
        }

        private void DrawCombo( CommandManager manager ) {
            using var combo = ImRaii.Combo( "##Combo", $"[{Value}] {CurrentTimeline}" );
            if( !combo ) return;

            foreach( var option in SheetData.WeaponTimelines ) {
                var selected = option.Key == Value;

                if( ImGui.Selectable( $"[{option.Key}] {option.Value}", selected ) ) {
                    SetValue( manager, option.Key );
                }

                if( selected ) ImGui.SetItemDefaultFocus();
            }
        }
    }
}
