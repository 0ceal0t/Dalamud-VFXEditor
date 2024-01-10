using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileManager.Interfaces;

namespace VfxEditor.Ui.Export.Penumbra {
    public class PenumbraGroup {
        private string Name = "New Group";
        private string Type = "Single";
        private int Priority = 0;

        private readonly List<PenumbraOption> Options = new();
        private readonly PenumbraOptionSplitView OptionView;

        public PenumbraGroup() {
            OptionView = new( Options );
        }

        public void Draw() {
            ImGui.InputTextWithHint( "##Name", "Name", ref Name, 255 );
            ImGui.InputInt( "Priority", ref Priority );

            if( ImGui.BeginCombo( "Type", Type ) ) {
                if( ImGui.Selectable( "Single" ) ) Type = "Single";
                if( ImGui.Selectable( "Multi" ) ) Type = "Multi";
                ImGui.EndCombo();
            }

            ImGui.Separator();

            OptionView.Draw();
        }

        public void RemoveDocument( IFileDocument document ) => Options.ForEach( x => x.RemoveDocument( document ) );

        public PenumbraGroupStruct Export( string modFolder ) {
            var defaultSettings = 0u;
            foreach( var (option, idx) in Options.WithIndex() ) {
                if( option.GetDefault() ) defaultSettings |= ( 1u << idx );
            }

            return new PenumbraGroupStruct {
                Name = Name,
                Priority = Priority,
                Type = Type,
                DefaultSettings = defaultSettings,
                Options = Options.Select( x => x.Export( modFolder, Name ) ).ToList()
            };
        }

        public void Reset() {
            Options.ForEach( x => x.Reset() );
            Options.Clear();
        }

        public string GetName() => Name;
    }
}
