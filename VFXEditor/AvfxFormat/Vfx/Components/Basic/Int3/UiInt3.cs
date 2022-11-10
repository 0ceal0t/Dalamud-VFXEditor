using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiInt3 : IUiBase {
        public readonly string Name;
        public readonly AVFXInt Literal1;
        public readonly AVFXInt Literal2;
        public readonly AVFXInt Literal3;
        private readonly List<AVFXBase> Literals = new();

        public UiInt3( string name, AVFXInt literal1, AVFXInt literal2, AVFXInt literal3 ) {
            Name = name;
            Literals.Add( Literal1 = literal1 );
            Literals.Add( Literal2 = literal2 );
            Literals.Add( Literal3 = literal3 );
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name + "_1"] = Literal1;
                CopyManager.Copied[Name + "_2"] = Literal2;
                CopyManager.Copied[Name + "_3"] = Literal3;
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Copied.TryGetValue( Name + "_1", out var _literal1 ) && _literal1 is AVFXInt literal1 ) {
                    Literal1.SetValue( literal1.GetValue() );
                    Literal1.SetAssigned( literal1.IsAssigned() );
                }
                if( CopyManager.Copied.TryGetValue( Name + "_2", out var _literal2 ) && _literal2 is AVFXInt literal2 ) {
                    Literal2.SetValue( literal2.GetValue() );
                    Literal2.SetAssigned( literal2.IsAssigned() );
                }
                if( CopyManager.Copied.TryGetValue( Name + "_3", out var _literal3 ) && _literal3 is AVFXInt literal3 ) {
                    Literal3.SetValue( literal3.GetValue() );
                    Literal3.SetAssigned( literal3.IsAssigned() );
                }
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literals, Name, id ) ) return;

            var value = new Vector3( Literal1.GetValue(), Literal2.GetValue(), Literal3.GetValue() );
            if( ImGui.InputFloat3( Name + id, ref value ) ) {
                CommandManager.Avfx.Add( new UiInt3Command( Literal1, Literal2, Literal3, value ) );
            }

            IUiBase.DrawRemoveContextMenu( Literals, Name, id );
        }
    }
}
