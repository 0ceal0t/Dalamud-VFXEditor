using ImGuiNET;
using System.Numerics;
using VFXEditor.AVFXLib;
using VFXEditor.Data;

namespace VFXEditor.AVFX.VFX {
    public class UIInt3 : UIBase {
        public readonly string Name;
        public Vector3 Value;

        public readonly AVFXInt Literal1;
        public readonly AVFXInt Literal2;
        public readonly AVFXInt Literal3;

        public UIInt3( string name, AVFXInt literal1, AVFXInt literal2, AVFXInt literal3 ) {
            Name = name;
            Literal1 = literal1;
            Literal2 = literal2;
            Literal3 = literal3;

            Value = new Vector3( Literal1.GetValue(), Literal2.GetValue(), Literal3.GetValue() );
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name + "_1"] = Literal1;
                CopyManager.Copied[Name + "_2"] = Literal2;
                CopyManager.Copied[Name + "_3"] = Literal3;
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Copied.TryGetValue( Name + "_1", out var _literal1 ) && _literal1 is AVFXInt literal1 ) {
                    Literal1.SetValue( literal1.GetValue() );
                    Literal1.SetAssigned( literal1.IsAssigned() );
                    Value.X = Literal1.GetValue();
                }
                if( CopyManager.Copied.TryGetValue( Name + "_2", out var _literal2 ) && _literal2 is AVFXInt literal2 ) {
                    Literal2.SetValue( literal2.GetValue() );
                    Literal2.SetAssigned( literal2.IsAssigned() );
                    Value.Y = Literal2.GetValue();
                }
                if( CopyManager.Copied.TryGetValue( Name + "_3", out var _literal3 ) && _literal3 is AVFXInt literal3 ) {
                    Literal3.SetValue( literal3.GetValue() );
                    Literal3.SetAssigned( literal3.IsAssigned() );
                    Value.Z = Literal3.GetValue();
                }
            }

            // Unassigned
            if (!Literal1.IsAssigned()) {
                if (ImGui.SmallButton($"+ {Name}{id}")) {
                    Literal1.SetAssigned( true );
                    Literal2.SetAssigned( true );
                    Literal3.SetAssigned( true );
                }
                return;
            }

            if( ImGui.InputFloat3( Name + id, ref Value ) ) {
                Literal1.SetValue( ( int )Value.X );
                Literal2.SetValue( ( int )Value.Y );
                Literal3.SetValue( ( int )Value.Z );
            }

            if( DrawUnassignContextMenu( id, Name ) ) {
                Literal1.SetAssigned( false );
                Literal2.SetAssigned( false );
                Literal3.SetAssigned( false );
            }
        }
    }
}
