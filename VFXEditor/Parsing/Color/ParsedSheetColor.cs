using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Data;

namespace VfxEditor.Parsing.Color {
    public class ParsedSheetColor : ParsedSimpleBase<uint, int> {
        private Vector4 CurrentColor => SheetData.UiColors.TryGetValue( Value, out var color ) ? color : new Vector4( 1, 1, 1, 1 );

        public ParsedSheetColor( string name ) : base( name ) {
            SheetData.InitUiColors();
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader, 0 );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadUInt32();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        public override bool Draw( CommandManager manager ) {
            var ret = Copy( manager );

            if( DrawCombo( manager ) ) ret = true;

            ImGui.SameLine();
            ImGui.Text( Name );

            return ret;
        }

        private bool DrawCombo( CommandManager manager ) {
            using var color = ImRaii.PushColor( ImGuiCol.Text, CurrentColor );
            var text = SheetData.UiColors.ContainsKey( Value ) ? $"----{Value}----" : "[NONE]";

            using var combo = ImRaii.Combo( "##Combo", text );
            if( !combo ) return false;

            var ret = false;

            foreach( var option in SheetData.UiColors ) {
                var selected = option.Key == Value;

                using var _ = ImRaii.PushColor( ImGuiCol.Text, option.Value );
                if( ImGui.Selectable( $"----{option.Key}----", selected ) ) {
                    manager.Add( new ParsedSimpleCommand<uint>( this, option.Key ) );
                    ret = true;
                }

                if( selected ) ImGui.SetItemDefaultFocus();
            }

            return ret;
        }

        protected override Dictionary<string, int> GetCopyMap( CopyManager manager ) => manager.Ints;

        protected override int ToCopy() => ( int )Value;

        protected override uint FromCopy( int val ) => ( uint )val;
    }
}
