using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileManager.Interfaces;

namespace VfxEditor.Ui.Export.Penumbra {
    public class PenumbraGroup {
        private string Name = "New Group";
        private string Type = "Single";
        private int Priority = 0;

        private readonly List<PenumbraOption> Options = [];
        private readonly PenumbraOptionSplitView OptionView;

        public PenumbraGroup() {
            OptionView = new( Options );
        }

        // Used for workspace imports
        public PenumbraGroup( PenumbraGroupStruct workspaceGroup, Dictionary<IFileManager, int> offsets ) : this() {
            Name = workspaceGroup.Name;
            Type = workspaceGroup.Type;
            Priority = workspaceGroup.Priority;

            foreach( var (option, idx) in workspaceGroup.Options.WithIndex() ) {
                var isDefault = ( ( workspaceGroup.DefaultSettings >> idx ) & 1u ) == 1u;
                Options.Add( new( option, isDefault, offsets ) );
            }
        }

        public void Draw() {
            ImGui.InputTextWithHint( "##Name", "Name", ref Name, 255 );

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SetNextItemWidth( 100 );
                if( ImGui.BeginCombo( "##Type", Type ) ) {
                    if( ImGui.Selectable( "Single" ) ) Type = "Single";
                    if( ImGui.Selectable( "Multi" ) ) Type = "Multi";
                    ImGui.EndCombo();
                }

                ImGui.SameLine();
                ImGui.SetNextItemWidth( 30 );
                ImGui.InputInt( "Priority", ref Priority, 0 );
            }

            ImGui.Separator();

            OptionView.Draw();
        }

        public void RemoveDocument( IFileDocument document ) => Options.ForEach( x => x.RemoveDocument( document ) );

        public PenumbraGroupStruct Export( string modFolder ) => new() {
            Name = Name,
            Priority = Priority,
            Type = Type,
            DefaultSettings = GetDefault(),
            Options = Options.Select( x => x.Export( modFolder, Name ) ).ToList()
        };

        public PenumbraGroupStruct WorkspaceExport() => new() {
            Name = Name,
            Priority = Priority,
            Type = Type,
            DefaultSettings = GetDefault(),
            Options = Options.Select( x => x.WorkspaceExport() ).ToList()
        };

        private uint GetDefault() {
            var defaultSettings = 0u;
            foreach( var (option, idx) in Options.WithIndex() ) {
                if( option.GetDefault() ) defaultSettings |= ( 1u << idx );
            }
            return defaultSettings;
        }

        public void Reset() {
            Options.ForEach( x => x.Reset() );
            Options.Clear();
        }

        public string GetName() => Name;


    }
}
