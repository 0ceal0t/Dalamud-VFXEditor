using ImGuiNET;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiInt : IUiBase {
        public readonly string Name;
        public readonly AVFXInt Literal;

        public UiInt( string name, AVFXInt literal ) {
            Name = name;
            Literal = literal;
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXInt literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
            }

            // Unassigned
            if( IUiBase.DrawCommandButton( Literal, Name, id ) ) return;

            var value = Literal.GetValue();
            if( ImGui.InputInt( Name + id, ref value ) ) {
                CommandManager.Avfx.Add( new UiIntCommand( Literal, value ) );
            }

            IUiBase.DrawCommandContextMenu( Literal, Name, id );
        }
    }
}
