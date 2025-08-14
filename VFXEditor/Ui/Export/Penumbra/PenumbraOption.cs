using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Ui.Export.Categories;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Ui.Export.Penumbra {
    public class PenumbraOption : IUiItem {
        private string Name = "New Option";
        private bool Default = false;
        private int Priority = 0;

        private readonly ExportDialogCategorySet CategorySet = new();

        public PenumbraOption() { }

        // Used for workspace imports
        public PenumbraOption( PenumbraOptionStruct workspaceOption, bool isDefault, Dictionary<IFileManager, int> offsets ) : this() {
            Name = workspaceOption.Name;
            Default = isDefault;
            Priority = workspaceOption.Priority;
            CategorySet.WorkspaceImport( workspaceOption.Files, offsets );
        }

        public void Draw() {
            ImGui.InputTextWithHint( "##Name", "Name", ref Name, 255 );

            ImGui.Checkbox( "Default", ref Default );

            ImGui.SameLine();
            ImGui.SetNextItemWidth( 30 );
            ImGui.InputInt( "Priority", ref Priority, 0 );

            ImGui.Separator();

            CategorySet.Draw();
        }

        public void RemoveDocument( IFileDocument document ) => CategorySet.RemoveDocument( document );

        public void Reset() => CategorySet.Reset();

        public PenumbraOptionStruct Export( string modFolder, string group ) => new() {
            Name = Name,
            Priority = Priority,
            Files = CategorySet.Export( modFolder, $"{group}/{Name}" )
        };

        public PenumbraOptionStruct WorkspaceExport() => new() {
            Name = Name,
            Priority = Priority,
            Files = CategorySet.WorkspaceExport()
        };

        public bool GetDefault() => Default;

        public string GetName() => Name;
    }
}
