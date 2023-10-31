using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Parsing.Int {
    public class ParsedItemSelect<T> : ParsedInt where T : class, ITextItem {
        private readonly List<T> Items;
        private readonly Func<T, int> ToValue;

        public ParsedItemSelect( List<T> items, Func<T, int> toValue, string name, int size = 4 ) : base( name, size ) {
            Items = items;
            ToValue = toValue;
        }

        public ParsedItemSelect( List<T> items, Func<T, int> toValue, string name, int value, int size = 4 ) : base( name, value, size ) {
            Items = items;
            ToValue = toValue;
        }

        protected override void DrawBody() {
            var selected = ToValue == null ?
                ( ( Value < 0 || Value >= Items.Count ) ? null : Items[Value] ) :
                Items.FirstOrDefault( x => ToValue( x ) == Value, null );

            using var combo = ImRaii.Combo( Name, selected == null ? "[NONE]" : selected.GetText() );
            if( !combo ) return;

            if( ImGui.Selectable( "[NONE]", selected == null ) ) CommandManager.Add( new ParsedSimpleCommand<int>( this, -1 ) );
            for( var i = 0; i < Items.Count; i++ ) {
                using var _ = ImRaii.PushId( i );
                if( ImGui.Selectable( Items[i].GetText(), Items[i] == selected ) ) {
                    SetValue( ToValue == null ? i : ToValue( Items[i] ) );
                }
            }
        }
    }
}
