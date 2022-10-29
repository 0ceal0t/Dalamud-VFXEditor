using ImGuiNET;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AVFX.VFX {
    public class UIInt : IUIBase {
        public readonly string Name;
        public int Value;
        public readonly AVFXInt Literal;

        public UIInt( string name, AVFXInt literal ) {
            Name = name;
            Literal = literal;
            Value = Literal.GetValue();
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXInt literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
                Value = Literal.GetValue();
            }

            // Unassigned
            if( !Literal.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {Name}{id}" ) ) Literal.SetAssigned( true );
                return;
            }

            if( ImGui.InputInt( Name + id, ref Value ) ) {
                Literal.SetValue( Value );
            }

            if( IUIBase.DrawUnassignContextMenu( id, Name ) ) Literal.SetAssigned( false );
        }
    }
}
