using System;
using ImGuiNET;
using AVFXLib.Models;
using System.Numerics;
using VFXEditor.Data;

namespace VFXEditor.Avfx.Vfx {
    public class UIFloat2 : UIBase {
        public string Name;
        public Vector2 Value;
        public LiteralFloat Literal1;
        public LiteralFloat Literal2;

        public UIFloat2( string name, LiteralFloat literal1, LiteralFloat literal2 ) {
            Name = name;
            Literal1 = literal1;
            Literal2 = literal2;
            Value = new Vector2( Literal1.Value, Literal2.Value );
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name + "_1"] = Literal1;
                CopyManager.Copied[Name + "_2"] = Literal2;
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Copied.TryGetValue( Name + "_1", out var b1 ) && b1 is LiteralFloat literal1 ) {
                    Literal1.GiveValue( literal1.Value );
                    Value.X = Literal1.Value;
                }
                if( CopyManager.Copied.TryGetValue( Name + "_2", out var b2 ) && b2 is LiteralFloat literal2 ) {
                    Literal2.GiveValue( literal2.Value );
                    Value.Y = Literal2.Value;
                }
            }

            if( ImGui.InputFloat2( Name + id, ref Value ) ) {
                Literal1.GiveValue( Value.X );
                Literal2.GiveValue( Value.Y );
            }
        }
    }
}
