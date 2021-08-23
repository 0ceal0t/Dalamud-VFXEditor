using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;
using VFXEditor.Data;

namespace VFXEditor.UI.VFX {
    public class UIString : UIBase {
        public string Name;
        public LiteralString Literal;
        public string Value;
        public uint MaxSize;
        public Action OnChange = null;

        public UIString( string name, LiteralString literal, Action onChange = null, int maxSizeBytes = 256 ) {
            Name = name;
            Literal = literal;
            Value = Literal.Value ?? "";
            MaxSize = ( uint )maxSizeBytes;
            OnChange = onChange;
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var b ) && b is LiteralString literal ) {
                Literal.GiveValue( literal.Value );
                Value = Literal.Value ?? "";
            }

            ImGui.InputText( Name + id, ref Value, MaxSize );
            ImGui.SameLine();
            if( ImGui.Button( "Update" + id ) ) {
                Literal.GiveValue( Value.Trim( '\0' ) + "\u0000" );
                OnChange?.Invoke();
            }
        }
    }
}
