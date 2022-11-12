using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiIntCombo : IUiBase {
        public readonly string Name;
        public readonly AVFXInt Literal;

        private readonly Dictionary<int, string> Mapping;
        private string DisplayText => Mapping.TryGetValue( Literal.GetValue(), out var displayText ) ? displayText : "[UNKNOWN]";

        public UiIntCombo( string name, AVFXInt literal, Dictionary<int, string> mapping ) {
            Name = name;
            Literal = literal;
            Mapping = mapping;
        }

        public void DrawInline( string id ) {
            // Copy/Paste
            if( CopyManager.IsCopying ) {
                CopyManager.Assigned[Name] = Literal.IsAssigned();
                CopyManager.Ints[Name] = Literal.GetValue();
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Assigned.TryGetValue( Name, out var a ) ) CopyManager.PasteCommand.Add( new UiAssignableCommand( Literal, a ) );
                if( CopyManager.Ints.TryGetValue( Name, out var l ) ) CopyManager.PasteCommand.Add( new UiIntCommand( Literal, l ) );
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literal, Name, id ) ) return;

            var value = Literal.GetValue();
            var spacing = ImGui.GetStyle().ItemSpacing.X;
            var comboWidth = ImGui.GetContentRegionAvail().X * 0.65f - 100 - spacing; // have to do this calculation now
            ImGui.SetNextItemWidth( 100 );
            if( ImGui.InputInt( $"{id}-MainInput", ref value ) ) {
                CommandManager.Avfx.Add( new UiIntCommand( Literal, value ) );
            }

            ImGui.SameLine(100 + spacing);
            ImGui.SetNextItemWidth( comboWidth );

            var idx = 0;
            if( ImGui.BeginCombo( $"{Name}{id}", DisplayText ) ) {
                foreach( var entry in Mapping ) {
                    var isSelected = entry.Key == value;
                    if( ImGui.Selectable( $"{entry.Value}##{idx}", isSelected ) ) {
                        CommandManager.Avfx.Add( new UiIntCommand( Literal, entry.Key ) );
                    }

                    if( isSelected ) ImGui.SetItemDefaultFocus();
                    idx++;
                }
                ImGui.EndCombo();
            }


            IUiBase.DrawRemoveContextMenu( Literal, Name, id );
        }
    }
}
