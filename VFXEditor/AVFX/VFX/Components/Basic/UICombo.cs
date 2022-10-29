using ImGuiNET;
using System;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AVFX.VFX {
    public class UICombo<T> : IUIBase {
        public readonly string Name;
        public int ValueIdx;
        public readonly AVFXEnum<T> Literal;
        public readonly Action OnChange;

        public UICombo( string name, AVFXEnum<T> literal, Action onChange = null ) {
            Name = name;
            Literal = literal;
            OnChange = onChange;
            ValueIdx = Array.IndexOf( Literal.Options, Literal.GetValue().ToString() );
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXEnum<T> literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
                ValueIdx = Array.IndexOf( Literal.Options, Literal.GetValue().ToString() );
                OnChange?.Invoke();
            }

            // Unassigned
            if( !Literal.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {Name}{id}" ) ) Literal.SetAssigned( true );
                return;
            }

            var text = ValueIdx == -1 ? "[NONE]" : Literal.Options[ValueIdx];
            if( ImGui.BeginCombo( Name + id, text ) ) {
                for( var i = 0; i < Literal.Options.Length; i++ ) {
                    var isSelected = ValueIdx == i;
                    if( ImGui.Selectable( Literal.Options[i], isSelected ) ) {
                        ValueIdx = i;
                        Literal.SetValue( Literal.Options[ValueIdx] );
                        OnChange?.Invoke();
                    }

                    if( isSelected ) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }

            if( IUIBase.DrawUnassignContextMenu( id, Name ) ) Literal.SetAssigned( false );
        }
    }
}
