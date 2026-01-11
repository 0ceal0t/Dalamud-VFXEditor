using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VFXEditor.Parsing {
    public class AssignableParsed<T> : IUiItem {
        private readonly ParsedSimpleBase<T> Parsed;
        public bool Assigned { get; private set; } = false;

        public T Value {
            get => Parsed.Value;
            set {
                Assigned = true;
                Parsed.Value = value;
            }
        }

        public AssignableParsed( ParsedSimpleBase<T> parsed ) {
            Parsed = parsed;
        }

        public void Draw() {
            var inputSize = UiUtils.GetOffsetInputSize( ImGui.GetFrameHeight() + ImGui.GetStyle().ItemSpacing.X );

            using var _ = ImRaii.PushId( Parsed.Name );
            var assigned = Assigned;
            if( ImGui.Checkbox( "##Assigned", ref assigned ) ) Assigned = assigned;
            using var disabled = ImRaii.Disabled( !Assigned );
            ImGui.SameLine();
            ImGui.SetNextItemWidth( inputSize );
            Parsed.Draw();
        }
    }
}
