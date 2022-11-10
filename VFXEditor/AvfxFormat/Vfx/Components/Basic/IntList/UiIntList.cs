using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiIntList : IUiBase {
        public readonly string Name;
        public readonly AVFXIntList Literal;

        public UiIntList( string name, AVFXIntList literal ) {
            Name = name;
            Literal = literal;
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXIntList literal ) {
                Literal.GetValue()[0] = literal.GetValue()[0];
                Literal.SetAssigned( literal.IsAssigned() );
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literal, Name, id ) ) return;

            var value = Literal.GetValue()[0];
            if( ImGui.InputInt( Name + id, ref value ) ) {
                CommandManager.Avfx.Add( new UiIntListCommand( Literal, value ) );
            }

            IUiBase.DrawRemoveContextMenu( Literal, Name, id );
        }
    }
}
