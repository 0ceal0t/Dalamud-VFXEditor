using System;
using System.Collections.Generic;
using ImGuiNET;
using VFXEditor.Data;
using VFXEditor.AVFXLib;

namespace VFXEditor.Avfx.Vfx {
    public class UIIntList : UIBase {
        public readonly string Name;
        public List<int> Value;
        public readonly AVFXIntList Literal;


        public UIIntList( string name, AVFXIntList literal ) {
            Name = name;
            Literal = literal;
            Value = Literal.GetValue();
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }

            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXIntList literal ) {
                Literal.GetValue()[0] = literal.GetValue()[0];
                Value[0] = Literal.GetValue()[0];
            }

            PushAssignedColor( Literal.IsAssigned() );
            var firstValue = Value[0];
            if( ImGui.InputInt( Name + id, ref firstValue ) ) {
                Literal.GetValue()[0] = firstValue;
                Value[0] = firstValue;
            }
            PopAssignedColor();
        }
    }
}
