using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Ui.Assignable {
    public class AssignableData<T> : IAssignable, IUiItem where T : class, IUiItem, new() {
        public readonly string Name;

        private bool Assigned = false;
        private T Data;

        public bool IsAssigned {
            get { return Assigned; }
            set { Assigned = value; }
        }

        public AssignableData( string name ) {
            Name = name;
        }

        public void Draw() {
            var open = false;
            using var _ = ImRaii.PushId( Name );
            using( var __ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                if( ImGui.Checkbox( $"##Assigned{Name}", ref Assigned ) ) {
                    Data ??= new();
                    CommandManager.Add( new AssignableCommand<T>( this ) );
                }

                using var disabled = ImRaii.Disabled( !Assigned );
                ImGui.SameLine();
                open = ImGui.CollapsingHeader( Name );
            }

            if( open && Assigned && Data != null ) {
                using var indent = ImRaii.PushIndent( 10f );
                Data.Draw();
            }
        }

        public void SetValue( T? value ) {
            if( value == null ) {
                Assigned = false;
            }
            else {
                Assigned = true;
                Data = value;
            }
        }

        public T GetValue() => Data;
    }
}
