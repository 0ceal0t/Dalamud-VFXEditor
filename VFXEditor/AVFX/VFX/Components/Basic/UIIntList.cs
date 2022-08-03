using ImGuiNET;
using System.Collections.Generic;
using VFXEditor.AVFXLib;
using VFXEditor.Data;

namespace VFXEditor.AVFX.VFX {
    public class UIIntList : IUIBase {
        public readonly string Name;
        public List<int> Value;
        public readonly AVFXIntList Literal;

        public UIIntList( string name, AVFXIntList literal ) {
            Name = name;
            Literal = literal;
            Value = Literal.GetValue();
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXIntList literal ) {
                Literal.GetValue()[0] = literal.GetValue()[0];
                Literal.SetAssigned( literal.IsAssigned() );
                Value[0] = Literal.GetValue()[0];
            }

            // Unassigned
            if( !Literal.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {Name}{id}" ) ) Literal.SetAssigned( true );
                return;
            }

            var firstValue = Value[0];
            if( ImGui.InputInt( Name + id, ref firstValue ) ) {
                Literal.GetValue()[0] = firstValue;
                Value[0] = firstValue;
            }

            if( IUIBase.DrawUnassignContextMenu( id, Name ) ) Literal.SetAssigned( false );
        }
    }
}
