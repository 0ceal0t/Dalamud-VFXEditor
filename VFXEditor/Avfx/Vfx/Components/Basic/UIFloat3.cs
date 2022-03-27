using System;
using ImGuiNET;
using AVFXLib.Models;
using System.Numerics;
using VFXEditor.Data;

namespace VFXEditor.Avfx.Vfx {
    public class UIFloat3 : UIBase {
        public string Name;
        public Vector3 Value;
        public LiteralFloat Literal1;
        public LiteralFloat Literal2;
        public LiteralFloat Literal3;

        public UIFloat3( string name, LiteralFloat literal1, LiteralFloat literal2, LiteralFloat literal3 ) {
            Name = name;
            Literal1 = literal1;
            Literal2 = literal2;
            Literal3 = literal3;

            Value = new Vector3( Literal1.Value, Literal2.Value, Literal3.Value );
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name + "_1"] = Literal1;
                CopyManager.Copied[Name + "_2"] = Literal2;
                CopyManager.Copied[Name + "_3"] = Literal3;
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
                if( CopyManager.Copied.TryGetValue( Name + "_3", out var b3 ) && b3 is LiteralFloat literal3 ) {
                    Literal3.GiveValue( literal3.Value );
                    Value.Z = Literal3.Value;
                }
            }

            if( ImGui.InputFloat3( Name + id, ref Value ) ) {
                Literal1.GiveValue( Value.X );
                Literal2.GiveValue( Value.Y );
                Literal3.GiveValue( Value.Z );
            }
        }
    }
}
