using ImGuiNET;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Ui.Export.Categories;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Ui.Export.Penumbra {
    public class PenumbraOptionView : IUiItem {
        private string Name = "New Option";
        private bool Default = false;
        private int Priority = 0;

        private readonly ExportDialogCategorySet CategorySet = new();

        public PenumbraOptionView() { }

        public void Draw() {
            ImGui.InputTextWithHint( "##Name", "Name", ref Name, 255 );
            ImGui.Checkbox( "Default", ref Default );
            ImGui.InputInt( "Priority", ref Priority );
            CategorySet.Draw();
        }

        public void RemoveDocument( IFileDocument document ) => CategorySet.RemoveDocument( document );

        public void Reset() => CategorySet.Reset();

        public PenumbraOptionStruct Export( string modFolder, string group ) {
            var option = new PenumbraOptionStruct {
                Name = Name,
                Priority = Priority,
                Files = CategorySet.Export( modFolder, $"{group}/{Name}" )
            };

            return option;
        }

        public bool GetDefault() => Default;

        public string GetName() => Name;
    }
}
