using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiIntCombo : IUiBase {
        public readonly string Name;
        public int Value;
        public readonly AVFXInt Literal;

        private readonly Dictionary<int, string> Mapping;
        private string DisplayText => Mapping.TryGetValue( Value, out var displayText ) ? displayText : "[UNKNOWN]";

        public UiIntCombo( string name, AVFXInt literal, Dictionary<int, string> mapping ) {
            Name = name;
            Literal = literal;
            Mapping = mapping;
            Value = Literal.GetValue();
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXInt literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
                Value = Literal.GetValue();
            }

            // Unassigned
            if( !Literal.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {Name}{id}" ) ) Literal.SetAssigned( true );
                return;
            }

            var spacing = ImGui.GetStyle().ItemSpacing.X;
            var comboWidth = ImGui.GetContentRegionAvail().X * 0.65f - 100 - spacing; // have to do this calculation now
            ImGui.SetNextItemWidth( 100 );
            if( ImGui.InputInt( $"{id}-MainInput", ref Value ) ) {
                Literal.SetValue( Value );
            }

            ImGui.SameLine(100 + spacing);
            ImGui.SetNextItemWidth( comboWidth );

            var idx = 0;
            if( ImGui.BeginCombo( $"{Name}{id}", DisplayText ) ) {
                foreach( var entry in Mapping ) {
                    var isSelected = entry.Key == Value;
                    if( ImGui.Selectable( $"{entry.Value}##{idx}", isSelected ) ) {
                        Value = entry.Key;
                        Literal.SetValue( Value );
                    }

                    if( isSelected ) ImGui.SetItemDefaultFocus();
                    idx++;
                }
                ImGui.EndCombo();
            }


            if( IUiBase.DrawUnassignContextMenu( id, Name ) ) Literal.SetAssigned( false );
        }
    }
}
