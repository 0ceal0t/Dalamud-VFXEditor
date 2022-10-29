using ImGuiNET;
using System.Numerics;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AVFX.VFX {
    public class UIFloat3 : IUIBase {
        public readonly string Name;
        public Vector3 Value;

        public readonly AVFXFloat Literal1;
        public readonly AVFXFloat Literal2;
        public readonly AVFXFloat Literal3;

        public UIFloat3( string name, AVFXFloat literal1, AVFXFloat literal2, AVFXFloat literal3 ) {
            Name = name;
            Literal1 = literal1;
            Literal2 = literal2;
            Literal3 = literal3;

            Value = new Vector3( Literal1.GetValue(), Literal2.GetValue(), Literal3.GetValue() );
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name + "_1"] = Literal1;
                CopyManager.Copied[Name + "_2"] = Literal2;
                CopyManager.Copied[Name + "_3"] = Literal3;
            }

            if( CopyManager.IsPasting ) {
                if( CopyManager.Copied.TryGetValue( Name + "_1", out var _literal1 ) && _literal1 is AVFXFloat literal1 ) {
                    Literal1.SetValue( literal1.GetValue() );
                    Literal1.SetAssigned( literal1.IsAssigned() );
                    Value.X = Literal1.GetValue();
                }
                if( CopyManager.Copied.TryGetValue( Name + "_2", out var _literal2 ) && _literal2 is AVFXFloat literal2 ) {
                    Literal2.SetValue( literal2.GetValue() );
                    Literal2.SetAssigned( literal2.IsAssigned() );
                    Value.Y = Literal2.GetValue();
                }
                if( CopyManager.Copied.TryGetValue( Name + "_3", out var _literal3 ) && _literal3 is AVFXFloat literal3 ) {
                    Literal3.SetValue( literal3.GetValue() );
                    Literal3.SetAssigned( literal3.IsAssigned() );
                    Value.Z = Literal3.GetValue();
                }
            }

            // Unassigned
            if( !Literal1.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {Name}{id}" ) ) {
                    Literal1.SetAssigned( true );
                    Literal2.SetAssigned( true );
                    Literal3.SetAssigned( true );
                }
                return;
            }

            if( ImGui.InputFloat3( Name + id, ref Value ) ) {
                Literal1.SetValue( Value.X );
                Literal2.SetValue( Value.Y );
                Literal3.SetValue( Value.Z );
            }

            if( IUIBase.DrawUnassignContextMenu( id, Name ) ) {
                Literal1.SetAssigned( false );
                Literal2.SetAssigned( false );
                Literal3.SetAssigned( false );
            }
        }
    }
}
