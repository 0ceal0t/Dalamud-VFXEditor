using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Linq;
using VfxEditor.Ui.Components.Base;
using VfxEditor.Utils;

namespace VfxEditor.Parsing.Int {
    public class ParsedIntSelect<T> : ParsedInt where T : class {
        private readonly Func<SelectView<T>> GetView;
        private readonly Func<T, int> ToValue;
        private readonly Func<T, int, string> GetText;
        private readonly int DefaultValue;

        public T Selected => ToValue == null ?
                ( ( Value < 0 || Value >= View.Items.Count ) ? null : View.Items[Value] ) :
                View.Items.FirstOrDefault( x => ToValue( x ) == Value, null );

        private SelectView<T> View => GetView();

        public ParsedIntSelect(
            string name, int defaultValue,
            Func<SelectView<T>> getView, Func<T, int> toValue, Func<T, int, string> getText,
            int size = 4 ) : base( name, size ) {

            GetView = getView;
            ToValue = toValue;
            GetText = getText;
            DefaultValue = defaultValue;
        }

        protected override void DrawBody() {
            ImGui.SetNextItemWidth( UiUtils.GetOffsetInputSize( FontAwesomeIcon.Share ) );
            using( var combo = ImRaii.Combo( $"##{Name}", Selected == null ? $"[NONE]: {Value}" : GetText( Selected, View.Items.IndexOf( Selected ) ) ) ) {
                if( combo ) {
                    if( ImGui.Selectable( $"[NONE]: {DefaultValue}", Selected == null ) ) CommandManager.Add( new ParsedSimpleCommand<int>( this, DefaultValue ) );
                    foreach( var (item, idx) in View.Items.WithIndex() ) {
                        using var _ = ImRaii.PushId( idx );
                        if( ImGui.Selectable( GetText( item, idx ), item == Selected ) ) Update( ToValue == null ? idx : ToValue( item ) );
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
