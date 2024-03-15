using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Linq;
using VfxEditor.Ui.Components.Base;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Parsing.Int {
    public class ParsedItemSelect<T> : ParsedInt where T : class, ITextItem {
        private readonly Func<SelectView<T>> GetView;
        private readonly Func<T, int> ToValue;
        private readonly int DefaultValue;

        private T Selected => ToValue == null ?
                ( ( Value < 0 || Value >= View.Items.Count ) ? null : View.Items[Value] ) :
                View.Items.FirstOrDefault( x => ToValue( x ) == Value, null );
        private SelectView<T> View => GetView();

        public ParsedItemSelect( string name, Func<SelectView<T>> getView, Func<T, int> toValue, int defaultValue, int size = 4 ) : base( name, size ) {
            GetView = getView;
            ToValue = toValue;
            DefaultValue = defaultValue;
        }

        protected override void DrawBody() {
            ImGui.SetNextItemWidth( UiUtils.GetOffsetInputSize( FontAwesomeIcon.Share ) );
            using( var combo = ImRaii.Combo( $"##{Name}", Selected == null ? "[NONE]" : Selected.GetText() ) ) {
                if( combo ) {
                    if( ImGui.Selectable( "[NONE]", Selected == null ) ) CommandManager.Add( new ParsedSimpleCommand<int>( this, DefaultValue ) );
                    foreach( var (item, idx) in View.Items.WithIndex() ) {
                        using var _ = ImRaii.PushId( idx );
                        if( ImGui.Selectable( item.GetText(), item == Selected ) ) Update( ToValue == null ? idx : ToValue( item ) );
                    }
                }
            }

            // ================================================

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
            ImGui.SameLine();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                using var dimmed = ImRaii.PushStyle( ImGuiStyleVar.Alpha, 0.5f, Selected == null );
                if( ImGui.Button( FontAwesomeIcon.Share.ToIconString() ) ) {
                    View.SetSelected( Selected );
                    UiUtils.ForceOpenTabs.Add( typeof( T ) );
                }
            }

            ImGui.SameLine();
            ImGui.Text( Name );
        }
    }
}
