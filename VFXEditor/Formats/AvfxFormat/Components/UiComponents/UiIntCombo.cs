using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Data.Copy;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiIntCombo : IUiItem {
        public readonly string Name;
        public readonly AvfxInt Literal;

        private readonly Dictionary<int, string> Mapping;
        private string DisplayText => Mapping.TryGetValue( Literal.Value, out var displayText ) ? displayText : "[UNKNOWN]";

        public UiIntCombo( string name, AvfxInt literal, Dictionary<int, string> mapping ) {
            Name = name;
            Literal = literal;
            Mapping = mapping;
        }

        public void Draw() {
            // Unassigned
            Literal.AssignedCopyPaste( Name );
            if( Literal.DrawAssignButton( Name ) ) return;

            // Copy/Paste
            CopyManager.TrySetValue( this, Name, Literal.Value );
            if( CopyManager.TryGetValue<int>( this, Name, out var val ) ) {
                CommandManager.Paste( new ParsedSimpleCommand<int>( Literal.Parsed, val ) );
            }

            var value = Literal.Value;
            var spacing = ImGui.GetStyle().ItemSpacing.X;
            var comboWidth = ImGui.GetContentRegionAvail().X * 0.65f - 100 - spacing;
            ImGui.SetNextItemWidth( 100 );
            if( ImGui.InputInt( "##MainInput", ref value ) ) CommandManager.Add( new ParsedSimpleCommand<int>( Literal.Parsed, value ) );

            ImGui.SameLine( 100 + spacing );
            ImGui.SetNextItemWidth( comboWidth );

            DrawCombo( value );

            Literal.DrawUnassignPopup( Name );
        }

        private void DrawCombo( int value ) {
            using var combo = ImRaii.Combo( Name, DisplayText );
            if( !combo ) return;

            var idx = 0;
            foreach( var entry in Mapping ) {
                using var _ = ImRaii.PushId( idx );
                var isSelected = entry.Key == value;
                if( ImGui.Selectable( $"{entry.Value}", isSelected ) ) {
                    CommandManager.Add( new ParsedSimpleCommand<int>( Literal.Parsed, entry.Key ) );
                }

                if( isSelected ) ImGui.SetItemDefaultFocus();
                idx++;
            }
        }
    }
}
