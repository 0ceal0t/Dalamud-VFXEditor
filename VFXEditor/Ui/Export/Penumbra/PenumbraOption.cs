using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Ui.Export.Categories;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Ui.Export.Penumbra {
    public class PenumbraOption : IUiItem {
        private readonly string Id = Guid.NewGuid().ToString();
        private string Name = "New Option";
        private bool Default = false;
        private int Priority = 0;
        private bool Hidden = false;

        private readonly ExportDialogCategorySet CategorySet = new();

        public PenumbraOption() { }

        // Used for workspace imports
        public PenumbraOption( PenumbraOptionStruct workspaceOption, bool isDefault, Dictionary<IFileManagerGroup, int> offsets ) : this() {
            Name = workspaceOption.Name;
            Default = isDefault;
            Priority = workspaceOption.Priority;
            Hidden = workspaceOption.Layout != null && workspaceOption.Layout.Contains( "Hide" );
            CategorySet.WorkspaceImport( workspaceOption.Files, offsets );
        }

        public void Draw() {
            ImGui.InputTextWithHint( "##Name", "Name", ref Name, 255 );

            ImGui.Checkbox( "Default", ref Default );

            ImGui.SameLine();
            ImGui.SetNextItemWidth( 30 );
            ImGui.InputInt( "Priority", ref Priority, 0 );

            ImGui.SameLine();
            ImGui.Checkbox( "Hidden", ref Hidden );
            if( ImGui.IsItemHovered() ) ImGui.SetTooltip( "Hides this option from Penumbra's own mod configuration window" );

            ImGui.Separator();

            CategorySet.Draw();
        }

        public void RemoveDocument( IFileDocument document ) => CategorySet.RemoveDocument( document );

        public void Reset() => CategorySet.Reset();

        public PenumbraOptionStructV4 Export( string modFolder, string group ) => new() {
            Id = Id,
            Name = Name,
            Priority = Priority,
            Layout = GetLayout(),
            Files = CategorySet.Export( modFolder, $"{group}/{Name}" )
        };

        public PenumbraOptionStruct WorkspaceExport() => new() {
            Name = Name,
            Priority = Priority,
            Layout = GetLayout(),
            Files = CategorySet.WorkspaceExport()
        };

        private List<string> GetLayout() => Hidden ? ["Hide"] : [];

        public bool GetDefault() => Default;

        public string GetName() => Name;
    }
}
