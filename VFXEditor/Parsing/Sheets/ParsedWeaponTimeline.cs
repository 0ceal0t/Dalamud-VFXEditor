using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor.Parsing.Sheets {
    public class ParsedWeaponTimeline : ParsedSimpleBase<ushort, int> {
        private string CurrentTimeline => SheetData.WeaponTimelines.TryGetValue( Value, out var timeline ) ? timeline : "";

        public ParsedWeaponTimeline( string name ) : base( name ) {
            SheetData.InitWeaponTimelines();
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader, 0 );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadUInt16();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        public override void Draw( CommandManager manager ) {
            Copy( manager );

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
                    manager.Add( new ParsedSimpleCommand<ushort>( this, option.Key ) );
                }

                if( selected ) ImGui.SetItemDefaultFocus();
            }
        }

        protected override Dictionary<string, int> GetCopyMap( CopyManager manager ) => manager.Ints;

        protected override int ToCopy() => Value;

        protected override ushort FromCopy( int val ) => ( ushort )val;
    }
}
