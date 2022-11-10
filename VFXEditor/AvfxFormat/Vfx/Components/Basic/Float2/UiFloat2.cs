using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiFloat2 : IUiBase {
        public readonly string Name;
        public readonly AVFXFloat Literal1;
        public readonly AVFXFloat Literal2;
        private readonly List<AVFXBase> Literals = new();

        public UiFloat2( string name, AVFXFloat literal1, AVFXFloat literal2 ) {
            Name = name;
            Literals.Add( Literal1 = literal1 );
            Literals.Add( Literal2 = literal2 );
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name + "_1"] = Literal1;
                CopyManager.Copied[Name + "_2"] = Literal2;
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Copied.TryGetValue( Name + "_1", out var _literal1 ) && _literal1 is AVFXFloat literal1 ) {
                    Literal1.SetValue( literal1.GetValue() );
                    Literal1.SetAssigned( literal1.IsAssigned() );
                }
                if( CopyManager.Copied.TryGetValue( Name + "_2", out var _literal2 ) && _literal2 is AVFXFloat literal2 ) {
                    Literal2.SetValue( literal2.GetValue() );
                    Literal2.SetAssigned( literal2.IsAssigned() );
                }
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literals, Name, id ) ) return;

            var value = new Vector2( Literal1.GetValue(), Literal2.GetValue() );
            if( ImGui.InputFloat2( Name + id, ref value ) ) {
                CommandManager.Avfx.Add( new UiFloat2Command( Literal1, Literal2, value ) );
            }

            IUiBase.DrawRemoveContextMenu( Literals, Name, id );
        }
    }
}
