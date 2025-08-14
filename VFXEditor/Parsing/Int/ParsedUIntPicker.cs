using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Parsing.Int {
    public class ParsedUIntPicker<T> : ParsedUIntHex where T : class {
        private readonly Func<List<T>> ListAction;
        private readonly Func<T, int, string> GetText;
        private readonly Func<T, uint> ToValue;

        public ParsedUIntPicker( string name, Func<List<T>> listAction, Func<T, int, string> getText, Func<T, uint> toValue ) : base( name, 4 ) {
            ListAction = listAction;
            GetText = getText;
            ToValue = toValue;
        }

        protected override void DrawBody() {
            var items = ListAction.Invoke();
            if( items == null ) {
                base.DrawBody();
                return;
            }

            var selected = Selected;
            var text = selected == null ? $"[NONE] 0x{Value:X8}" : GetText( selected, items.IndexOf( selected ) );

            using var combo = ImRaii.Combo( Name, text );
            if( !combo ) return;
            foreach( var (item, idx) in items.WithIndex() ) {
                using var _ = ImRaii.PushId( idx );
                if( ImGui.Selectable( GetText( item, idx ), item == selected ) ) {
                    Value = ToValue == null ? ( uint )idx : ToValue( item );
                }
                if( item == selected ) ImGui.SetItemDefaultFocus();
            }
        }

        public T Selected {
            get {
                var items = ListAction.Invoke();
                if( items == null ) return null;

                return ToValue == null ?
                    ( ( Value < 0 || Value >= items.Count ) ? null : items[( int )Value] ) :
                    items.FirstOrDefault( x => ToValue( x ) == Value, null ); ;
            }
        }
    }
}