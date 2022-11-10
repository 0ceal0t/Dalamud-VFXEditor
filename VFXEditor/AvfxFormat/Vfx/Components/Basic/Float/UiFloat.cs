using ImGuiNET;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiFloat : IUiBase {
        public readonly string Name;
        public readonly AVFXFloat Literal;

        public UiFloat( string name, AVFXFloat literal ) {
            Name = name;
            Literal = literal;
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXFloat literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literal, Name, id ) ) return;

            var value = Literal.GetValue();
            if( ImGui.InputFloat( Name + id, ref value ) ) {
                CommandManager.Avfx.Add( new UiFloatCommand( Literal, value ) );
            }

            IUiBase.DrawRemoveContextMenu( Literal, Name, id );
        }
    }
}
