using ImGuiNET;
using OtterGui;
using OtterGui.Raii;
using System;
using System.Collections.Generic;

namespace VfxEditor.Parsing.Int {
    public class ParsedUIntPicker<T> : ParsedUIntHex where T : class {
        private readonly Func<IEnumerable<T>> ListAction;
        private readonly Func<T, string> TextAction;
        private readonly Func<T, uint> ValueAction;

        public ParsedUIntPicker( string name, Func<IEnumerable<T>> listAction, Func<T, string> textAction, Func<T, uint> valueAction ) : base( name, 4 ) {
            ListAction = listAction;
            TextAction = textAction;
            ValueAction = valueAction;
        }

        protected override void DrawBody() {
            var items = ListAction.Invoke();
            if( items == null ) {
                base.DrawBody();
                return;
            }

            var found = items.FindFirst( x => ValueAction( x ) == Value, out var _selected );
            var selected = found ? _selected! : null;

            var text = selected == null ? $"[UNKNOWN]" : TextAction( selected );
            using var combo = ImRaii.Combo( Name, text );
            if( !combo ) return;

            foreach( var (item, idx) in items.WithIndex() ) {
                using var _ = ImRaii.PushId( idx );
                if( ImGui.Selectable( TextAction( item ), item == selected ) ) {
                    Value = ValueAction( item );
                }
                if( item == selected ) ImGui.SetItemDefaultFocus();
            }
        }
    }
}