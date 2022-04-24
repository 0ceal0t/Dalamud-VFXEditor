using ImGuiNET;
using System.Numerics;
using VFXEditor.AVFXLib;
using VFXEditor.Data;

namespace VFXEditor.AVFX.VFX {
    public class UIFloat2 : UIBase {
        public readonly string Name;
        public Vector2 Value;
        public readonly AVFXFloat Literal1;
        public readonly AVFXFloat Literal2;

        public UIFloat2( string name, AVFXFloat literal1, AVFXFloat literal2 ) {
            Name = name;
            Literal1 = literal1;
            Literal2 = literal2;
            Value = new Vector2( Literal1.GetValue(), Literal2.GetValue() );
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name + "_1"] = Literal1;
                CopyManager.Copied[Name + "_2"] = Literal2;
            }

            if( CopyManager.IsPasting ) {
                if( CopyManager.Copied.TryGetValue( Name + "_1", out var _literal1 ) && _literal1 is AVFXFloat literal1 ) {
                    Literal1.SetValue( literal1.GetValue() );
                    Value.X = Literal1.GetValue();
                }
                if( CopyManager.Copied.TryGetValue( Name + "_2", out var _literal2 ) && _literal2 is AVFXFloat literal2 ) {
                    Literal2.SetValue( literal2.GetValue() );
                    Value.Y = Literal2.GetValue();
                }
            }

            PushAssignedColor( Literal1.IsAssigned() );
            if( ImGui.InputFloat2( Name + id, ref Value ) ) {
                Literal1.SetValue( Value.X );
                Literal2.SetValue( Value.Y );
            }
            PopAssignedColor();
        }
    }
}
