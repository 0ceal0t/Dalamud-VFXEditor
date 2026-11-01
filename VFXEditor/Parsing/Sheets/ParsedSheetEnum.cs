using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;

namespace VfxEditor.Parsing {
    public abstract class ParsedSheetEnum<T> : ParsedSimpleBase<T> where T : notnull {
        private string Current => SheetData.TryGetValue( Value, out var timeline ) ? timeline : "";
        private readonly Dictionary<T, string> SheetData;

        public ParsedSheetEnum( string name, Dictionary<T, string> sheetData, Action initData ) : base( name ) {
            SheetData = sheetData;
            initData();
        }

        protected override void DrawBody() {
            DrawCombo();
            ImGui.SameLine();
            ImGui.Text( Name );
        }

        private void DrawCombo() {
            using var combo = ImRaii.Combo( "##Combo", $"[{Value}] {Current}" );
            if( !combo ) return;

            foreach( var option in SheetData ) {
                var selected = option.Key.Equals( Value );

                if( ImGui.Selectable( $"[{option.Key}] {option.Value}", selected ) ) {
                    Update( option.Key );
                }

                if( selected ) ImGui.SetItemDefaultFocus();
            }
        }
    }
}