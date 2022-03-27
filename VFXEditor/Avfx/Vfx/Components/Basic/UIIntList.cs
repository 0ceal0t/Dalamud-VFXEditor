using System;
using System.Collections.Generic;
using ImGuiNET;
using AVFXLib.Models;
using VFXEditor.Data;

namespace VFXEditor.Avfx.Vfx {
    public class UIIntList : UIBase {
        public string Name;
        public List<int> Value;
        public LiteralIntList Literal;


        public UIIntList( string name, LiteralIntList literal ) {
            Name = name;
            Literal = literal;
            Value = Literal.Value;
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var b ) && b is LiteralIntList literal ) {
                Literal.Value[0] = literal.Value[0];
                Value[0] = Literal.Value[0];
            }

            var v0 = Value[0];
            if( ImGui.InputInt( Name + id, ref v0 ) ) {
                Literal.Value[0] = v0;
                Value[0] = v0;
            }
        }
    }
}
