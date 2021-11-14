using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;
using Dalamud.Plugin;
using VFXEditor.Data;

namespace VFXEditor.Avfx.Vfx {
    public class UICombo<T> : UIBase {
        public string Name;
        public int ValueIdx;
        public LiteralEnum<T> Literal;

        public Action OnChange = null;

        public UICombo( string name, LiteralEnum<T> literal, Action onChange = null ) {
            Name = name;
            Literal = literal;
            OnChange = onChange;
            ValueIdx = Array.IndexOf( Literal.Options, Literal.Value.ToString() );
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var b ) && b is LiteralEnum<T> literal ) {
                Literal.GiveValue( literal.Value );
                ValueIdx = Array.IndexOf( Literal.Options, Literal.Value.ToString() );
                OnChange?.Invoke();
            }

            var text = ValueIdx == -1 ? "[NONE]" : Literal.Options[ValueIdx];
            if( ImGui.BeginCombo( Name + id, text ) ) {
                for( var i = 0; i < Literal.Options.Length; i++ ) {
                    var isSelected = ( ValueIdx == i );
                    if( ImGui.Selectable( Literal.Options[i], isSelected ) ) {
                        ValueIdx = i;
                        Literal.GiveValue( Literal.Options[ValueIdx] );
                        OnChange?.Invoke();
                    }
                    if( isSelected ) {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
        }
    }
}
