using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;

namespace VfxEditor.Parsing.HalfFloat {
    public class ParsedHalf3Color : ParsedHalf3 {
        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private Vector3 StateBeforeEdit;

        public ParsedHalf3Color( string name ) : base( name ) { }

        public ParsedHalf3Color( string name, Vector3 value ) : base( name, value ) { }

        protected override void DrawBody() {
            var prevValue = Value;
            if( ImGui.ColorEdit3( Name, ref Value, ImGuiColorEditFlags.Float | ImGuiColorEditFlags.NoDragDrop ) ) {
                if( !Editing ) {
                    Editing = true;
                    StateBeforeEdit = prevValue;
                }
                LastEditTime = DateTime.Now;
            }
            else if( Editing && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) {
                Editing = false;
                Update( StateBeforeEdit, Value );
            }
        }

        public void DrawPreview() {
            ImGui.ColorButton( $"##{Name}", new Vector4( Value, 1f ),
                ImGuiColorEditFlags.NoOptions |
                ImGuiColorEditFlags.NoDragDrop |
                ImGuiColorEditFlags.NoAlpha |
                ImGuiColorEditFlags.NoPicker |
                ImGuiColorEditFlags.NoTooltip );
        }
    }
}
